using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LauncherBuilderCS.Controls
{
    internal sealed class SkinButtonEditor : GroupBox
    {
        private readonly NumericUpDown _xNumeric;
        private readonly NumericUpDown _yNumeric;
        private readonly TextBox _normalTextBox;
        private readonly Button _normalBrowseButton;
        private readonly TextBox _downTextBox;
        private readonly Button _downBrowseButton;
        private readonly OpenFileDialog _fileDialog = new()
        {
            Filter = "Image files|*.bmp;*.png;*.jpg;*.jpeg|All files|*.*",
            RestoreDirectory = true
        };

        private SkinButtonAsset? _asset;
        private bool _updating;

        public SkinButtonEditor(string title)
        {
            Text = title;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(12);
            Margin = new Padding(3);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            layout.Controls.Add(new Label { Text = "X position:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
            _xNumeric = CreateNumericUpDown();
            layout.Controls.Add(_xNumeric, 1, 0);
            layout.SetColumnSpan(_xNumeric, 2);

            layout.Controls.Add(new Label { Text = "Y position:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
            _yNumeric = CreateNumericUpDown();
            layout.Controls.Add(_yNumeric, 1, 1);
            layout.SetColumnSpan(_yNumeric, 2);

            layout.Controls.Add(new Label { Text = "Normal image:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
            _normalTextBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            layout.Controls.Add(_normalTextBox, 1, 2);
            _normalBrowseButton = new Button { Text = "Browse...", AutoSize = true };
            layout.Controls.Add(_normalBrowseButton, 2, 2);

            layout.Controls.Add(new Label { Text = "Pressed image:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
            _downTextBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            layout.Controls.Add(_downTextBox, 1, 3);
            _downBrowseButton = new Button { Text = "Browse...", AutoSize = true };
            layout.Controls.Add(_downBrowseButton, 2, 3);

            Controls.Add(layout);

            _xNumeric.ValueChanged += (_, _) => UpdateAssetPosition();
            _yNumeric.ValueChanged += (_, _) => UpdateAssetPosition();

            _normalTextBox.TextChanged += (_, _) => UpdateAssetPath(isNormal: true, _normalTextBox.Text);
            _downTextBox.TextChanged += (_, _) => UpdateAssetPath(isNormal: false, _downTextBox.Text);

            _normalBrowseButton.Click += (_, _) => BrowseForImage(isNormal: true);
            _downBrowseButton.Click += (_, _) => BrowseForImage(isNormal: false);
        }

        public void Bind(SkinButtonAsset asset)
        {
            _asset = asset ?? throw new ArgumentNullException(nameof(asset));
            RefreshValues();
        }

        public void RefreshValues()
        {
            if (_asset == null)
            {
                return;
            }

            _updating = true;
            try
            {
                _xNumeric.Value = Clamp(_xNumeric, _asset.X);
                _yNumeric.Value = Clamp(_yNumeric, _asset.Y);
                _normalTextBox.Text = _asset.NormalPath ?? string.Empty;
                _downTextBox.Text = _asset.DownPath ?? string.Empty;
            }
            finally
            {
                _updating = false;
            }
        }

        public void SetImagePickersEnabled(bool enabled)
        {
            _normalTextBox.Enabled = enabled;
            _normalBrowseButton.Enabled = enabled;
            _downTextBox.Enabled = enabled;
            _downBrowseButton.Enabled = enabled;
        }

        private static NumericUpDown CreateNumericUpDown()
        {
            return new NumericUpDown
            {
                Minimum = -2048,
                Maximum = 4096,
                Increment = 1,
                Width = 120
            };
        }

        private void UpdateAssetPosition()
        {
            if (_asset == null || _updating)
            {
                return;
            }

            _asset.X = (int)_xNumeric.Value;
            _asset.Y = (int)_yNumeric.Value;
        }

        private void UpdateAssetPath(bool isNormal, string value)
        {
            if (_asset == null || _updating)
            {
                return;
            }

            if (isNormal)
            {
                _asset.NormalPath = value;
            }
            else
            {
                _asset.DownPath = value;
            }
        }

        private void BrowseForImage(bool isNormal)
        {
            if (_asset == null)
            {
                return;
            }

            var current = isNormal ? _asset.NormalPath : _asset.DownPath;
            if (!string.IsNullOrWhiteSpace(current) && File.Exists(current))
            {
                _fileDialog.InitialDirectory = Path.GetDirectoryName(current);
                _fileDialog.FileName = Path.GetFileName(current);
            }
            else
            {
                _fileDialog.FileName = string.Empty;
            }

            if (_fileDialog.ShowDialog(this) == DialogResult.OK)
            {
                _updating = true;
                try
                {
                    if (isNormal)
                    {
                        _asset.NormalPath = _fileDialog.FileName;
                        _normalTextBox.Text = _fileDialog.FileName;
                    }
                    else
                    {
                        _asset.DownPath = _fileDialog.FileName;
                        _downTextBox.Text = _fileDialog.FileName;
                    }
                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private static decimal Clamp(NumericUpDown numeric, int value)
        {
            var min = numeric.Minimum;
            var max = numeric.Maximum;
            var clamped = Math.Min((int)max, Math.Max((int)min, value));
            return clamped;
        }
    }
}
