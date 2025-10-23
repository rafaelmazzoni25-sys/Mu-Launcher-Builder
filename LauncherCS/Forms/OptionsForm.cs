using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace LauncherCS.Forms
{
    public sealed class OptionsForm : Form
    {
        private readonly LauncherConfiguration _configuration;
        private readonly TextBox _accountId;
        private readonly List<RadioButton> _resolutionButtons = new();
        private readonly CheckBox _soundCheck;
        private readonly CheckBox _musicCheck;
        private readonly ImageButton _closeButton;
        private readonly ImageButton _applyButton;
        private Bitmap? _background;
        private Region? _region;
        private bool _dragging;
        private Point _dragStart;
        private readonly List<Image> _ownedImages = new();

        public OptionsForm(LauncherConfiguration configuration)
        {
            _configuration = configuration;

            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = configuration.Options.OptionsColor;
            ForeColor = configuration.Options.OptionsFontColor;

            var idLabel = new Label { AutoSize = false, TextAlign = ContentAlignment.MiddleLeft };
            var resLabel = new Label { AutoSize = false, TextAlign = ContentAlignment.MiddleLeft };
            var soundLabel = new Label { AutoSize = false, TextAlign = ContentAlignment.MiddleLeft };

            idLabel.BackColor = Color.Transparent;
            resLabel.BackColor = Color.Transparent;
            soundLabel.BackColor = Color.Transparent;
            idLabel.ForeColor = ForeColor;
            resLabel.ForeColor = ForeColor;
            soundLabel.ForeColor = ForeColor;

            _accountId = new TextBox();

            _resolutionButtons.Add(new RadioButton { Text = "640x480" });
            _resolutionButtons.Add(new RadioButton { Text = "800x600" });
            _resolutionButtons.Add(new RadioButton { Text = "1024x768" });
            _resolutionButtons.Add(new RadioButton { Text = "1280x1024" });

            _soundCheck = new CheckBox { Text = "SOUND ON/OFF" };
            _musicCheck = new CheckBox { Text = "MUSIC ON/OFF" };

            Controls.Add(idLabel);
            Controls.Add(resLabel);
            Controls.Add(soundLabel);
            Controls.Add(_accountId);
            foreach (var rb in _resolutionButtons)
            {
                Controls.Add(rb);
            }
            Controls.Add(_soundCheck);
            Controls.Add(_musicCheck);

            _closeButton = CreateButton(configuration.Skin.Buttons.Close2, CloseButton_Click);
            _applyButton = CreateButton(configuration.Skin.Buttons.Apply, ApplyButton_Click);

            Controls.Add(_closeButton);
            Controls.Add(_applyButton);

            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            Paint += OnPaint;
            FormClosed += (_, _) => CleanUp();

            ApplySkin(idLabel, resLabel, soundLabel);
            LoadBackground();
            LoadRegistrySettings();
        }

        private void ApplySkin(Label idLabel, Label resLabel, Label soundLabel)
        {
            var skin = _configuration.Skin;

            idLabel.Text = skin.Id.Id;
            idLabel.Left = skin.Id.IdText.Left;
            idLabel.Top = skin.Id.IdText.Top;
            idLabel.Width = skin.Id.IdText.Width;
            idLabel.Height = skin.Id.IdText.Height;

            _accountId.Left = skin.Id.IdPos.Left;
            _accountId.Top = skin.Id.IdPos.Top;
            _accountId.Width = skin.Id.IdPos.Width;
            _accountId.Height = skin.Id.IdPos.Height;

            resLabel.Text = skin.Res.Resolution;
            resLabel.Left = skin.Res.ResolutionText.Left;
            resLabel.Top = skin.Res.ResolutionText.Top;
            resLabel.Width = skin.Res.ResolutionText.Width;
            resLabel.Height = skin.Res.ResolutionText.Height;

            var resPositions = new[]
            {
                skin.Res.ResOptions.Res1,
                skin.Res.ResOptions.Res2,
                skin.Res.ResOptions.Res3,
                skin.Res.ResOptions.Res4
            };

            for (int i = 0; i < _resolutionButtons.Count; i++)
            {
                var target = resPositions[i];
                var rb = _resolutionButtons[i];
                rb.Left = target.Left;
                rb.Top = target.Top;
                rb.AutoSize = true;
                rb.BackColor = Color.Transparent;
                rb.ForeColor = ForeColor;
            }

            soundLabel.Text = skin.Sound.Sound;
            soundLabel.Left = skin.Sound.SoundText.Left;
            soundLabel.Top = skin.Sound.SoundText.Top;
            soundLabel.Width = skin.Sound.SoundText.Width;
            soundLabel.Height = skin.Sound.SoundText.Height;

            _soundCheck.Left = skin.Sound.SoundOptions.Sound.Left;
            _soundCheck.Top = skin.Sound.SoundOptions.Sound.Top;
            _soundCheck.AutoSize = true;
            _soundCheck.BackColor = Color.Transparent;
            _soundCheck.ForeColor = ForeColor;

            _musicCheck.Left = skin.Sound.SoundOptions.Music.Left;
            _musicCheck.Top = skin.Sound.SoundOptions.Music.Top;
            _musicCheck.AutoSize = true;
            _musicCheck.BackColor = Color.Transparent;
            _musicCheck.ForeColor = ForeColor;

            _closeButton.Left = skin.Buttons.Close2.X;
            _closeButton.Top = skin.Buttons.Close2.Y;

            _applyButton.Left = skin.Buttons.Apply.X;
            _applyButton.Top = skin.Buttons.Apply.Y;
        }

        private void LoadBackground()
        {
            _background?.Dispose();
            _region?.Dispose();
            _region = null;
            Region = null;

            _background = ResourceDecoder.LoadBitmap(_configuration.Skin.OptionBackground);
            ClientSize = _background.Size;

            if (!string.Equals(_configuration.Skin.OptionRegion, "EMPTY", StringComparison.OrdinalIgnoreCase))
            {
                using var optionRegionBitmap = ResourceDecoder.LoadBitmap(_configuration.Skin.OptionRegion);
                _region = ResourceDecoder.CreateRegion(optionRegionBitmap);
                Region = _region;
            }
        }

        private ImageButton CreateButton(SkinButton skinButton, EventHandler onClick)
        {
            var button = new ImageButton();
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

            button.Click += onClick;
            return button;
        }

        private void LoadRegistrySettings()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\\Webzen\\Mu\\Config");
                if (key != null)
                {
                    _accountId.Text = key.GetValue("ID", string.Empty)?.ToString() ?? string.Empty;
                    var colorDepth = Convert.ToInt32(key.GetValue("ColorDepth", 0));
                    if (colorDepth >= 0 && colorDepth < _resolutionButtons.Count)
                    {
                        _resolutionButtons[colorDepth].Checked = true;
                    }

                    _soundCheck.Checked = Convert.ToInt32(key.GetValue("SoundOnOff", 0)) != 0;
                    _musicCheck.Checked = Convert.ToInt32(key.GetValue("MusicOnOff", 0)) != 0;
                }
            }
            catch
            {
                // Ignore registry errors to mirror original behaviour
            }

            if (_resolutionButtons.TrueForAll(rb => !rb.Checked))
            {
                _resolutionButtons[0].Checked = true;
            }
        }

        private void SaveRegistrySettings()
        {
            try
            {
                using var key = Registry.CurrentUser.CreateSubKey(@"Software\\Webzen\\Mu\\Config");
                if (key == null)
                {
                    throw new InvalidOperationException("Unable to open registry key.");
                }

                key.SetValue("ID", _accountId.Text, RegistryValueKind.String);

                var selected = 0;
                for (var i = 0; i < _resolutionButtons.Count; i++)
                {
                    if (_resolutionButtons[i].Checked)
                    {
                        selected = i;
                        break;
                    }
                }

                key.SetValue("ColorDepth", selected, RegistryValueKind.DWord);
                key.SetValue("SoundOnOff", _soundCheck.Checked ? 1 : 0, RegistryValueKind.DWord);
                key.SetValue("MusicOnOff", _musicCheck.Checked ? 1 : 0, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save configuration: {ex.Message}", "Launcher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ApplyButton_Click(object? sender, EventArgs e)
        {
            SaveRegistrySettings();
            Close();
        }

        private void CloseButton_Click(object? sender, EventArgs e) => Close();

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
            if (_background != null)
            {
                e.Graphics.DrawImageUnscaled(_background, Point.Empty);
            }
        }

        private void CleanUp()
        {
            _closeButton.Dispose();
            _applyButton.Dispose();
            foreach (var image in _ownedImages)
            {
                image.Dispose();
            }
            _ownedImages.Clear();
            _background?.Dispose();
            _region?.Dispose();
            _region = null;
        }
    }
}
