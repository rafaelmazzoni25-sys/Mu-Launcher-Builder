using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using LauncherBuilderCS.Services;

namespace LauncherBuilderCS.Controls
{
    internal sealed class UpdateBuilderControl : UserControl
    {
        private readonly TextBox _inputTextBox;
        private readonly TextBox _outputTextBox;
        private readonly Button _browseInputButton;
        private readonly Button _browseOutputButton;
        private readonly Button _buildButton;
        private readonly ProgressBar _progressBar;
        private readonly Label _statusLabel;
        private readonly FolderBrowserDialog _folderDialog = new();
        private readonly UpdatePackageBuilder _builder = new();

        public UpdateBuilderControl()
        {
            Dock = DockStyle.Fill;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 5,
                Padding = new Padding(10),
                AutoSize = true
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var inputLabel = new Label
            {
                Text = "Input folder:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            _inputTextBox = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            _browseInputButton = new Button
            {
                Text = "Browse...",
                AutoSize = true
            };
            _browseInputButton.Click += (_, _) => BrowseForFolder(_inputTextBox);

            var outputLabel = new Label
            {
                Text = "Output folder:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            _outputTextBox = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            _browseOutputButton = new Button
            {
                Text = "Browse...",
                AutoSize = true
            };
            _browseOutputButton.Click += (_, _) => BrowseForFolder(_outputTextBox);

            _buildButton = new Button
            {
                Text = "Create update package",
                AutoSize = true,
                Anchor = AnchorStyles.Right
            };
            _buildButton.Click += async (sender, args) => await BuildAsync();

            _progressBar = new ProgressBar
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                Minimum = 0,
                Maximum = 1,
                Value = 0
            };

            _statusLabel = new Label
            {
                Text = "Idle.",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            layout.Controls.Add(inputLabel, 0, 0);
            layout.Controls.Add(_inputTextBox, 1, 0);
            layout.Controls.Add(_browseInputButton, 2, 0);

            layout.Controls.Add(outputLabel, 0, 1);
            layout.Controls.Add(_outputTextBox, 1, 1);
            layout.Controls.Add(_browseOutputButton, 2, 1);

            layout.Controls.Add(_buildButton, 2, 2);

            layout.SetColumnSpan(_progressBar, 3);
            layout.Controls.Add(_progressBar, 0, 3);

            layout.SetColumnSpan(_statusLabel, 3);
            layout.Controls.Add(_statusLabel, 0, 4);

            Controls.Add(layout);
        }

        private void BrowseForFolder(TextBox target)
        {
            if (_folderDialog.ShowDialog(this) == DialogResult.OK)
            {
                target.Text = _folderDialog.SelectedPath;
            }
        }

        private async Task BuildAsync()
        {
            var input = _inputTextBox.Text;
            var output = _outputTextBox.Text;

            if (string.IsNullOrWhiteSpace(input) || !Directory.Exists(input))
            {
                MessageBox.Show(this, "Please select a valid input folder containing the files to package.", "Update Builder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(output))
            {
                MessageBox.Show(this, "Please select the folder where the package will be created.", "Update Builder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ToggleControls(false);
            _progressBar.Value = 0;
            _progressBar.Maximum = 1;
            _statusLabel.Text = "Packing files...";

            try
            {
                var progress = new Progress<UpdateProgress>(UpdateProgress);
                await Task.Run(() => _builder.Build(input, output, progress));
                _statusLabel.Text = "Update package created successfully.";
                _progressBar.Value = _progressBar.Maximum;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to create the update package: {ex.Message}", "Update Builder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _statusLabel.Text = "Failed to create update package.";
            }
            finally
            {
                ToggleControls(true);
            }
        }

        private void UpdateProgress(UpdateProgress progress)
        {
            if (progress.Total > 0)
            {
                if (_progressBar.Maximum != progress.Total)
                {
                    _progressBar.Maximum = progress.Total;
                }

                var value = Math.Clamp(progress.Completed, 0, _progressBar.Maximum);
                _progressBar.Value = value;
            }
            else
            {
                _progressBar.Maximum = 1;
                _progressBar.Value = 1;
            }

            _statusLabel.Text = $"Packing {progress.CurrentFile}";
        }

        private void ToggleControls(bool enabled)
        {
            _inputTextBox.Enabled = enabled;
            _outputTextBox.Enabled = enabled;
            _browseInputButton.Enabled = enabled;
            _browseOutputButton.Enabled = enabled;
            _buildButton.Enabled = enabled;
        }
    }
}
