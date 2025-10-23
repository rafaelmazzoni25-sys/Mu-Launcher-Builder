using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LauncherBuilderCS.Controls
{
    internal sealed class GeneralOptionsControl : UserControl
    {
        private sealed record CheckBinding(CheckBox CheckBox, Func<GeneralOptions, bool> Getter, Action<GeneralOptions, bool> Setter);
        private sealed record TextBinding(TextBox TextBox, Func<GeneralOptions, string> Getter, Action<GeneralOptions, string> Setter);

        private readonly List<CheckBinding> _checkBindings = new();
        private readonly List<TextBinding> _textBindings = new();

        private readonly OpenFileDialog _imageDialog = new()
        {
            Filter = "Image files|*.bmp;*.png;*.jpg;*.jpeg|All files|*.*",
            RestoreDirectory = true
        };

        private GeneralOptions? _options;
        private bool _updating;

        private readonly CheckBox _showAnnouncementCheckBox;
        private readonly TextBox _announcementUrlTextBox;
        private readonly CheckBox _showOptionsCheckBox;
        private readonly CheckBox _showUpdateCheckBox;
        private readonly CheckBox _showServerStatusCheckBox;
        private readonly CheckBox _showSplashCheckBox;
        private readonly TextBox _splashBackgroundTextBox;
        private readonly Button _splashBackgroundBrowseButton;
        private readonly TextBox _splashRegionTextBox;
        private readonly Button _splashRegionBrowseButton;
        private readonly TextBox _serverNameTextBox;
        private readonly TextBox _serverPageTextBox;
        private readonly TextBox _serverHostTextBox;
        private readonly TextBox _serverPortTextBox;
        private readonly TextBox _updateUrlTextBox;

        public GeneralOptionsControl()
        {
            Dock = DockStyle.Fill;
            AutoScroll = true;
            BackColor = Color.FromArgb(245, 246, 248);

            var container = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(12)
            };
            container.Resize += (_, _) => ExpandGroupBoxes(container);

            _showAnnouncementCheckBox = new CheckBox { Text = "Enable announcement browser", AutoSize = true };
            _announcementUrlTextBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };

            var browserGroup = CreateGroupBox("Announcement Browser");
            var browserLayout = CreateTableLayout();
            browserLayout.Controls.Add(_showAnnouncementCheckBox, 0, 0);
            browserLayout.SetColumnSpan(_showAnnouncementCheckBox, 3);
            browserLayout.Controls.Add(new Label { Text = "Browser URL:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
            browserLayout.Controls.Add(_announcementUrlTextBox, 1, 1);
            browserLayout.SetColumnSpan(_announcementUrlTextBox, 2);
            browserGroup.Controls.Add(browserLayout);

            _showOptionsCheckBox = new CheckBox { Text = "Enable options window", AutoSize = true };
            _showUpdateCheckBox = new CheckBox { Text = "Show update button", AutoSize = true };
            _showServerStatusCheckBox = new CheckBox { Text = "Display server status", AutoSize = true };

            var optionsGroup = CreateGroupBox("Main Window Buttons");
            var optionsLayout = CreateTableLayout();
            optionsLayout.Controls.Add(_showOptionsCheckBox, 0, 0);
            optionsLayout.SetColumnSpan(_showOptionsCheckBox, 3);
            optionsLayout.Controls.Add(_showUpdateCheckBox, 0, 1);
            optionsLayout.SetColumnSpan(_showUpdateCheckBox, 3);
            optionsLayout.Controls.Add(_showServerStatusCheckBox, 0, 2);
            optionsLayout.SetColumnSpan(_showServerStatusCheckBox, 3);
            optionsGroup.Controls.Add(optionsLayout);

            _showSplashCheckBox = new CheckBox { Text = "Enable splash screen", AutoSize = true };
            _splashBackgroundTextBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            _splashBackgroundBrowseButton = new Button { Text = "Browse...", AutoSize = true };
            _splashRegionTextBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            _splashRegionBrowseButton = new Button { Text = "Browse...", AutoSize = true };

            _splashBackgroundBrowseButton.Click += (_, _) => BrowseForImage(_splashBackgroundTextBox, (options, path) => options.SplashBackgroundPath = path);
            _splashRegionBrowseButton.Click += (_, _) => BrowseForImage(_splashRegionTextBox, (options, path) => options.SplashRegionPath = path);

            var splashGroup = CreateGroupBox("Splash Screen");
            var splashLayout = CreateTableLayout();
            splashLayout.Controls.Add(_showSplashCheckBox, 0, 0);
            splashLayout.SetColumnSpan(_showSplashCheckBox, 3);
            splashLayout.Controls.Add(new Label { Text = "Background image:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
            splashLayout.Controls.Add(_splashBackgroundTextBox, 1, 1);
            splashLayout.Controls.Add(_splashBackgroundBrowseButton, 2, 1);
            splashLayout.Controls.Add(new Label { Text = "Region bitmap:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
            splashLayout.Controls.Add(_splashRegionTextBox, 1, 2);
            splashLayout.Controls.Add(_splashRegionBrowseButton, 2, 2);
            splashGroup.Controls.Add(splashLayout);

            _serverNameTextBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            _serverPageTextBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            _serverHostTextBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            _serverPortTextBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };

            var serverGroup = CreateGroupBox("Server Information");
            var serverLayout = CreateTableLayout();
            serverLayout.Controls.Add(new Label { Text = "Server name:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
            serverLayout.Controls.Add(_serverNameTextBox, 1, 0);
            serverLayout.Controls.Add(new Label { Text = "Server page:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 1);
            serverLayout.Controls.Add(_serverPageTextBox, 1, 1);
            serverLayout.Controls.Add(new Label { Text = "Hostname or IP:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 2);
            serverLayout.Controls.Add(_serverHostTextBox, 1, 2);
            serverLayout.Controls.Add(new Label { Text = "Port:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
            serverLayout.Controls.Add(_serverPortTextBox, 1, 3);
            serverGroup.Controls.Add(serverLayout);

            _updateUrlTextBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };

            var updateGroup = CreateGroupBox("Update Packages");
            var updateLayout = CreateTableLayout();
            updateLayout.Controls.Add(new Label { Text = "Package index URL:", AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
            updateLayout.Controls.Add(_updateUrlTextBox, 1, 0);
            updateGroup.Controls.Add(updateLayout);

            container.Controls.Add(browserGroup);
            container.Controls.Add(optionsGroup);
            container.Controls.Add(splashGroup);
            container.Controls.Add(serverGroup);
            container.Controls.Add(updateGroup);

            Controls.Add(container);

            RegisterCheckBinding(_showAnnouncementCheckBox, o => o.ShowAnnouncement, (o, value) => o.ShowAnnouncement = value);
            RegisterTextBinding(_announcementUrlTextBox, o => o.AnnouncementUrl, (o, value) => o.AnnouncementUrl = value);

            RegisterCheckBinding(_showOptionsCheckBox, o => o.ShowOptionsWindow, (o, value) => o.ShowOptionsWindow = value);
            RegisterCheckBinding(_showUpdateCheckBox, o => o.ShowUpdateButton, (o, value) => o.ShowUpdateButton = value);
            RegisterCheckBinding(_showServerStatusCheckBox, o => o.ShowServerStatus, (o, value) => o.ShowServerStatus = value);

            RegisterCheckBinding(_showSplashCheckBox, o => o.ShowSplashScreen, (o, value) => o.ShowSplashScreen = value);
            RegisterTextBinding(_splashBackgroundTextBox, o => o.SplashBackgroundPath, (o, value) => o.SplashBackgroundPath = value);
            RegisterTextBinding(_splashRegionTextBox, o => o.SplashRegionPath, (o, value) => o.SplashRegionPath = value);

            RegisterTextBinding(_serverNameTextBox, o => o.ServerName, (o, value) => o.ServerName = value);
            RegisterTextBinding(_serverPageTextBox, o => o.ServerPage, (o, value) => o.ServerPage = value);
            RegisterTextBinding(_serverHostTextBox, o => o.ServerHost, (o, value) => o.ServerHost = value);
            RegisterTextBinding(_serverPortTextBox, o => o.ServerPort, (o, value) => o.ServerPort = value);

            RegisterTextBinding(_updateUrlTextBox, o => o.UpdateUrl, (o, value) => o.UpdateUrl = value);
        }

        public void Bind(GeneralOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            RefreshValues();
        }

        public void RefreshValues()
        {
            if (_options == null)
            {
                return;
            }

            _updating = true;
            try
            {
                foreach (var binding in _checkBindings)
                {
                    binding.CheckBox.Checked = binding.Getter(_options);
                }

                foreach (var binding in _textBindings)
                {
                    binding.TextBox.Text = binding.Getter(_options) ?? string.Empty;
                }
            }
            finally
            {
                _updating = false;
            }

            UpdateEnabledStates();
        }

        private void RegisterCheckBinding(CheckBox checkBox, Func<GeneralOptions, bool> getter, Action<GeneralOptions, bool> setter)
        {
            checkBox.CheckedChanged += (_, _) =>
            {
                if (_options != null && !_updating)
                {
                    setter(_options, checkBox.Checked);
                    UpdateEnabledStates();
                }
            };
            _checkBindings.Add(new CheckBinding(checkBox, getter, setter));
        }

        private void RegisterTextBinding(TextBox textBox, Func<GeneralOptions, string> getter, Action<GeneralOptions, string> setter)
        {
            textBox.TextChanged += (_, _) =>
            {
                if (_options != null && !_updating)
                {
                    setter(_options, textBox.Text);
                }
            };
            _textBindings.Add(new TextBinding(textBox, getter, setter));
        }

        private static GroupBox CreateGroupBox(string title)
        {
            return new GroupBox
            {
                Text = title,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(12),
                Margin = new Padding(3)
            };
        }

        private static TableLayoutPanel CreateTableLayout()
        {
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

            return layout;
        }

        private void BrowseForImage(TextBox target, Action<GeneralOptions, string> setter)
        {
            if (_options == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(target.Text) && File.Exists(target.Text))
            {
                _imageDialog.InitialDirectory = Path.GetDirectoryName(target.Text);
                _imageDialog.FileName = Path.GetFileName(target.Text);
            }
            else
            {
                _imageDialog.FileName = string.Empty;
            }

            if (_imageDialog.ShowDialog(this) == DialogResult.OK)
            {
                _updating = true;
                try
                {
                    target.Text = _imageDialog.FileName;
                    setter(_options, _imageDialog.FileName);
                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private void UpdateEnabledStates()
        {
            var enableBrowserFields = _showAnnouncementCheckBox.Checked;
            _announcementUrlTextBox.Enabled = enableBrowserFields;

            var enableSplashFields = _showSplashCheckBox.Checked;
            _splashBackgroundTextBox.Enabled = enableSplashFields;
            _splashBackgroundBrowseButton.Enabled = enableSplashFields;
            _splashRegionTextBox.Enabled = enableSplashFields;
            _splashRegionBrowseButton.Enabled = enableSplashFields;
        }

        private static void ExpandGroupBoxes(FlowLayoutPanel panel)
        {
            var width = panel.ClientSize.Width - panel.Padding.Horizontal;
            foreach (Control control in panel.Controls)
            {
                if (control is GroupBox group)
                {
                    group.Width = Math.Max(100, width - group.Margin.Horizontal);
                }
            }
        }
    }
}
