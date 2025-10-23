using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LauncherCS.Forms
{
    public sealed class MainForm : Form
    {
        private readonly LauncherConfiguration _configuration;
        private Bitmap? _backgroundImage;
        private Region? _windowRegion;
        private ImageButton? _btnClose;
        private ImageButton? _btnConnect;
        private ImageButton? _btnUpdate;
        private ImageButton? _btnOption;
        private WebBrowser? _browser;
        private readonly Label _statusLabel;
        private readonly Timer _statusTimer;
        private bool _dragging;
        private Point _dragStart;
        private readonly List<Image> _ownedImages = new();

        public MainForm(LauncherConfiguration configuration)
        {
            _configuration = configuration;

            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            Text = $"{configuration.Options.ServerName} - {configuration.Options.ServerPage}";
            BackColor = configuration.Options.MainColor;
            ForeColor = configuration.Options.MainFontColor;

            _statusLabel = new Label
            {
                Visible = false,
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(_statusLabel);

            _statusTimer = new Timer { Interval = 5000 };
            _statusTimer.Tick += OnStatusTimerTick;

            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            FormClosed += OnFormClosed;
            Paint += OnPaint;

            Load += (_, _) => ApplyConfiguration();
        }

        private void ApplyConfiguration()
        {
            try
            {
                LoadBackground();
                SetupBrowser();
                SetupButtons();
                SetupStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize launcher: {ex.Message}", "Launcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void LoadBackground()
        {
            _backgroundImage?.Dispose();
            _windowRegion?.Dispose();
            _windowRegion = null;
            Region = null;

            _backgroundImage = ResourceDecoder.LoadBitmap(_configuration.Skin.MainBackground);
            ClientSize = _backgroundImage.Size;

            if (!string.Equals(_configuration.Skin.MainRegion, "EMPTY", StringComparison.OrdinalIgnoreCase))
            {
                using var regionBitmap = ResourceDecoder.LoadBitmap(_configuration.Skin.MainRegion);
                _windowRegion = ResourceDecoder.CreateRegion(regionBitmap);
                Region = _windowRegion;
            }
        }

        private void SetupBrowser()
        {
            if (!_configuration.Options.ShowBrowse)
            {
                return;
            }

            _browser = new WebBrowser
            {
                Left = _configuration.Skin.Browser.Left,
                Top = _configuration.Skin.Browser.Top,
                Width = _configuration.Skin.Browser.Width,
                Height = _configuration.Skin.Browser.Height
            };
            if (!string.IsNullOrWhiteSpace(_configuration.Options.BrowseUrl))
            {
                try
                {
                    _browser.Navigate(_configuration.Options.BrowseUrl);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to navigate browser: {ex.Message}", "Launcher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            Controls.Add(_browser);
        }

        private void SetupButtons()
        {
            _btnClose = CreateButton(_configuration.Skin.Buttons.Close, CloseButton_Click);
            _btnConnect = CreateButton(_configuration.Skin.Buttons.Connect, ConnectButton_Click);

            if (_configuration.Options.ShowUpdate)
            {
                _btnUpdate = CreateButton(_configuration.Skin.Buttons.Update, UpdateButton_Click);
            }

            if (_configuration.Options.ShowOption)
            {
                _btnOption = CreateButton(_configuration.Skin.Buttons.Option, OptionButton_Click);
            }
        }

        private void SetupStatus()
        {
            if (!_configuration.Options.ShowStatus)
            {
                return;
            }

            _statusLabel.Left = _configuration.Skin.ServerStatus.Left;
            _statusLabel.Width = _configuration.Skin.ServerStatus.Width;
            _statusLabel.Top = _configuration.Skin.ServerStatus.Top;
            _statusLabel.Height = _configuration.Skin.ServerStatus.Height;
            _statusLabel.ForeColor = ForeColor;
            _statusLabel.Visible = true;
            _statusTimer.Start();
        }

        private ImageButton CreateButton(SkinButton skinButton, EventHandler handler)
        {
            var button = new ImageButton
            {
                Left = skinButton.X,
                Top = skinButton.Y
            };

            if (!string.Equals(skinButton.DataNormal, "EMPTY", StringComparison.OrdinalIgnoreCase))
            {
                var normal = ResourceDecoder.LoadBitmap(skinButton.DataNormal);
                button.NormalImage = normal;
                _ownedImages.Add(normal);
            }

            if (!string.Equals(skinButton.DataDown, "EMPTY", StringComparison.OrdinalIgnoreCase))
            {
                var down = ResourceDecoder.LoadBitmap(skinButton.DataDown);
                button.DownImage = down;
                _ownedImages.Add(down);
            }

            button.Click += handler;
            Controls.Add(button);
            return button;
        }

        private void CloseButton_Click(object? sender, EventArgs e) => Close();

        private void OptionButton_Click(object? sender, EventArgs e)
        {
            using var options = new OptionsForm(_configuration);
            options.ShowDialog(this);
        }

        private void UpdateButton_Click(object? sender, EventArgs e)
        {
            using var updater = new UpdateForm(_configuration.Options.UpdateData);
            updater.ShowDialog(this);
        }

        private void ConnectButton_Click(object? sender, EventArgs e)
        {
            if (_configuration.Options.ShowSplash)
            {
                using var splash = new SplashForm(_configuration);
                Hide();
                splash.ShowDialog(this);
                Show();
            }

            var mainExe = Path.Combine(AppContext.BaseDirectory, "main.exe");
            if (!File.Exists(mainExe))
            {
                MessageBox.Show($"Unable to find {mainExe}.", "Launcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var parameters = $"connect /u{_configuration.Options.ServerIp} /p{_configuration.Options.ServerPort}";
                Process.Start(new ProcessStartInfo
                {
                    FileName = mainExe,
                    Arguments = parameters,
                    WorkingDirectory = AppContext.BaseDirectory,
                    UseShellExecute = true
                });
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start main.exe: {ex.Message}", "Launcher", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void OnStatusTimerTick(object? sender, EventArgs e)
        {
            _statusTimer.Stop();
            try
            {
                if (!int.TryParse(_configuration.Options.ServerPort, out var port))
                {
                    _statusLabel.Text = "Invalid server port";
                    return;
                }

                var online = await CheckGameServerAsync(_configuration.Options.ServerIp, port);
                if (online)
                {
                    var latency = await PingServerAsync(_configuration.Options.ServerIp);
                    _statusLabel.Text = $"Online - {latency}";
                    _statusTimer.Start();
                }
                else
                {
                    _statusLabel.Text = "Server is offline";
                }
            }
            catch (Exception ex)
            {
                _statusLabel.Text = $"Status error: {ex.Message}";
            }
        }

        private static async Task<bool> CheckGameServerAsync(string host, int port)
        {
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(host, port);
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(2));
                var completed = await Task.WhenAny(connectTask, timeoutTask).ConfigureAwait(false);
                if (completed != connectTask || !client.Connected)
                {
                    return false;
                }

                client.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static async Task<string> PingServerAsync(string host)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(host, 1000).ConfigureAwait(false);
                return reply.Status switch
                {
                    IPStatus.Success => $"{reply.RoundtripTime}ms",
                    IPStatus.TimedOut => "Time Out",
                    IPStatus.DestinationHostUnreachable => "Host Unreachable",
                    IPStatus.TtlExpired => "TTL Exceeded",
                    _ => "Error"
                };
            }
            catch
            {
                return "No Route to host";
            }
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = true;
                _dragStart = PointToScreen(e.Location);
            }
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            if (!_dragging)
            {
                return;
            }

            var current = PointToScreen(e.Location);
            var diff = new Point(current.X - _dragStart.X, current.Y - _dragStart.Y);
            Location = new Point(Location.X + diff.X, Location.Y + diff.Y);
            _dragStart = current;
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragging = false;
            }
        }

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            if (_backgroundImage != null)
            {
                e.Graphics.DrawImageUnscaled(_backgroundImage, Point.Empty);
            }
        }

        private void OnFormClosed(object? sender, FormClosedEventArgs e)
        {
            _statusTimer.Stop();
            _statusTimer.Dispose();
            _browser?.Dispose();
            _btnClose?.Dispose();
            _btnConnect?.Dispose();
            _btnUpdate?.Dispose();
            _btnOption?.Dispose();
            _backgroundImage?.Dispose();
            _windowRegion?.Dispose();
            _windowRegion = null;
            foreach (var image in _ownedImages)
            {
                image.Dispose();
            }
            _ownedImages.Clear();
        }
    }
}
