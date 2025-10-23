using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LauncherCS.Forms
{
    public sealed class UpdateForm : Form
    {
        private readonly HttpClient _httpClient = new();
        private readonly ProgressBar _fileProgress;
        private readonly ProgressBar _totalProgress;
        private readonly ListBox _log;
        private readonly StatusStrip _statusStrip;
        private readonly ToolStripStatusLabel _statusLabel;
        private readonly Button _cancelButton;
        private readonly Button _updateButton;
        private CancellationTokenSource? _cts;
        private readonly string _updateBaseUrl;

        public UpdateForm(string updateBaseUrl)
        {
            _updateBaseUrl = string.IsNullOrWhiteSpace(updateBaseUrl) ? string.Empty : EnsureTrailingSlash(updateBaseUrl);

            Text = "Launcher Update";
            Width = 600;
            Height = 420;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            _fileProgress = new ProgressBar { Left = 16, Top = 16, Width = 550, Height = 24 };
            _totalProgress = new ProgressBar { Left = 16, Top = 56, Width = 550, Height = 24 };
            _log = new ListBox { Left = 16, Top = 96, Width = 550, Height = 220 };
            _cancelButton = new Button { Left = 376, Top = 330, Width = 90, Height = 28, Text = "Cancel" };
            _updateButton = new Button { Left = 476, Top = 330, Width = 90, Height = 28, Text = "Update" };

            _cancelButton.Click += CancelButton_Click;
            _updateButton.Click += UpdateButton_Click;

            _statusLabel = new ToolStripStatusLabel { Text = "Idle" };
            _statusStrip = new StatusStrip();
            _statusStrip.Items.Add(_statusLabel);

            Controls.AddRange(new Control[]
            {
                _fileProgress,
                _totalProgress,
                _log,
                _cancelButton,
                _updateButton,
                _statusStrip
            });
        }

        private static string EnsureTrailingSlash(string url) => url.EndsWith("/") ? url : url + "/";

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            if (_cts == null)
            {
                Close();
                return;
            }

            _cts.Cancel();
            _statusLabel.Text = "Cancelled.";
        }

        private async void UpdateButton_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_updateBaseUrl))
            {
                MessageBox.Show("Update URL is not configured.", "Launcher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _updateButton.Enabled = false;
            _cts = new CancellationTokenSource();
            _log.Items.Clear();
            _statusLabel.Text = "Downloading update data...";

            try
            {
                await RunUpdateAsync(_cts.Token);
                _statusLabel.Text = "Finished";
            }
            catch (OperationCanceledException)
            {
                _statusLabel.Text = "Cancelled.";
            }
            catch (Exception ex)
            {
                _statusLabel.Text = "Error";
                Log($"Exception encountered: {ex.Message}");
            }
            finally
            {
                _updateButton.Enabled = true;
                _cts?.Dispose();
                _cts = null;
            }
        }

        private async Task RunUpdateAsync(CancellationToken token)
        {
            var updateConfigUrl = _updateBaseUrl + "update.cfg";
            var updateData = await DownloadBytesAsync(updateConfigUrl, null, token);

            using var updateStream = new MemoryStream(updateData);
            ResourceDecoder.EncryptDecrypt(updateStream);
            using var unpacked = ResourceDecoder.UnpackToMemory(updateStream);
            using var reader = new StreamReader(unpacked, Encoding.UTF8);
            var fileLines = reader.ReadToEnd()
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            if (fileLines.Count == 0)
            {
                Log("No update entries found.");
                return;
            }

            InvokeUI(() =>
            {
                _totalProgress.Value = 0;
                _totalProgress.Maximum = fileLines.Count;
            });

            var currentPath = AppContext.BaseDirectory;
            using var md5 = MD5.Create();

            for (var index = 0; index < fileLines.Count; index++)
            {
                token.ThrowIfCancellationRequested();

                var parts = fileLines[index].Split(',');
                if (parts.Length < 2)
                {
                    continue;
                }

                var fileName = parts[0].Trim();
                var expectedHash = parts[1].Trim().ToLowerInvariant();
                var destinationPath = Path.Combine(currentPath, fileName.Replace('/', Path.DirectorySeparatorChar));

                bool needsDownload = true;
                if (File.Exists(destinationPath))
                {
                    using var fileStream = File.OpenRead(destinationPath);
                    var existingHash = BitConverter.ToString(md5.ComputeHash(fileStream)).Replace("-", string.Empty).ToLowerInvariant();
                    needsDownload = !string.Equals(existingHash, expectedHash, StringComparison.OrdinalIgnoreCase);
                }

                if (needsDownload)
                {
                    var downloadUrl = ResourceDecoder.BuildUrl(_updateBaseUrl + "udata/", fileName + ".pak");
                    await DownloadAndInstallAsync(downloadUrl, destinationPath, token);
                }

                InvokeUI(() => _totalProgress.Value = Math.Min(index + 1, _totalProgress.Maximum));
            }
        }

        private async Task DownloadAndInstallAsync(string url, string destination, CancellationToken token)
        {
            Log($"Downloading {Path.GetFileName(destination)}");
            var fileBytes = await DownloadBytesAsync(url, _fileProgress, token);

            using var downloadStream = new MemoryStream(fileBytes);
            using var unpacked = ResourceDecoder.UnpackToMemory(downloadStream);
            unpacked.Position = 0;
            using var reader = new StreamReader(unpacked);
            var base64 = reader.ReadToEnd().Trim();
            var decoded = Convert.FromBase64String(base64);

            var directory = Path.GetDirectoryName(destination);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllBytesAsync(destination, decoded, token);
        }

        private async Task<byte[]> DownloadBytesAsync(string url, ProgressBar? progressBar, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var total = response.Content.Headers.ContentLength ?? -1L;

            if (progressBar != null)
            {
                InvokeUI(() =>
                {
                    progressBar.Value = 0;
                    progressBar.Maximum = total > 0 && total < int.MaxValue ? (int)total : int.MaxValue;
                });
            }

            using var stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
            var buffer = new byte[81920];
            int read;
            long totalRead = 0;
            using var ms = new MemoryStream();
            while ((read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), token).ConfigureAwait(false)) > 0)
            {
                await ms.WriteAsync(buffer.AsMemory(0, read), token).ConfigureAwait(false);
                totalRead += read;
                if (progressBar != null && total > 0 && total < int.MaxValue)
                {
                    var value = (int)Math.Min(totalRead, progressBar.Maximum);
                    InvokeUI(() => progressBar.Value = value);
                }
            }

            if (progressBar != null)
            {
                InvokeUI(() => progressBar.Value = progressBar.Maximum);
            }

            return ms.ToArray();
        }

        private void Log(string message)
        {
            InvokeUI(() => _log.Items.Add(message));
        }

        private void InvokeUI(Action action)
        {
            if (InvokeRequired)
            {
                BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient.Dispose();
                _cts?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
