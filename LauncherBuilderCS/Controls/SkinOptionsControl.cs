using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LauncherBuilderCS.Controls
{
    internal sealed class SkinOptionsControl : UserControl
    {
        private sealed record CheckBinding(CheckBox CheckBox, Func<SkinOptions, bool> Getter, Action<SkinOptions, bool> Setter);
        private sealed record TextBinding(TextBox TextBox, Func<SkinOptions, string> Getter, Action<SkinOptions, string> Setter);
        private sealed record NumericBinding(NumericUpDown Numeric, Func<SkinOptions, int> Getter, Action<SkinOptions, int> Setter);
        private sealed record ColorBinding(Button Button, Panel Preview, Label HexLabel, Func<SkinOptions, Color> Getter, Action<SkinOptions, Color> Setter);
        private sealed record FilePicker(TextBox TextBox, Button Button);

        private readonly List<CheckBinding> _checkBindings = new();
        private readonly List<TextBinding> _textBindings = new();
        private readonly List<NumericBinding> _numericBindings = new();
        private readonly List<ColorBinding> _colorBindings = new();
        private readonly List<FilePicker> _filePickers = new();

        private readonly ColorDialog _colorDialog = new() { FullOpen = true, AnyColor = true };
        private readonly OpenFileDialog _imageDialog = new()
        {
            Filter = "Image files|*.bmp;*.png;*.jpg;*.jpeg|All files|*.*",
            RestoreDirectory = true
        };

        private SkinOptions? _options;
        private bool _updating;

        private readonly CheckBox _useCustomSkinCheckBox;

        private readonly SkinButtonEditor _closeButtonEditor;
        private readonly SkinButtonEditor _connectButtonEditor;
        private readonly SkinButtonEditor _updateButtonEditor;
        private readonly SkinButtonEditor _optionsButtonEditor;
        private readonly SkinButtonEditor _secondaryCloseButtonEditor;
        private readonly SkinButtonEditor _applyButtonEditor;

        public SkinOptionsControl()
        {
            Dock = DockStyle.Fill;
            AutoScroll = true;
            BackColor = Color.FromArgb(245, 246, 248);

            var tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };

            var appearancePage = new TabPage("Appearance") { AutoScroll = true, Padding = new Padding(6) };
            var buttonsPage = new TabPage("Buttons") { AutoScroll = true, Padding = new Padding(6) };
            var layoutPage = new TabPage("Layout & Text") { AutoScroll = true, Padding = new Padding(6) };

            tabControl.TabPages.Add(appearancePage);
            tabControl.TabPages.Add(buttonsPage);
            tabControl.TabPages.Add(layoutPage);

            Controls.Add(tabControl);

            var appearancePanel = CreateFlowPanel();
            appearancePage.Controls.Add(appearancePanel);

            var buttonsPanel = CreateFlowPanel();
            buttonsPage.Controls.Add(buttonsPanel);

            var layoutPanel = CreateFlowPanel();
            layoutPage.Controls.Add(layoutPanel);

            _useCustomSkinCheckBox = new CheckBox { Text = "Use custom skin", AutoSize = true };
            var generalGroup = CreateGroupBox("Skin selection");
            var generalLayout = CreateSingleColumnLayout();
            generalLayout.Controls.Add(new Label { Text = "Provide custom images to override the default launcher skin.", AutoSize = true, Anchor = AnchorStyles.Left });
            generalLayout.Controls.Add(_useCustomSkinCheckBox);
            generalGroup.Controls.Add(generalLayout);
            appearancePanel.Controls.Add(generalGroup);

            var colorsGroup = CreateGroupBox("Window colors");
            var colorsLayout = CreateColorTable();
            var (mainColorButton, mainColorPreview, mainColorLabel) = AddColorRow(colorsLayout, "Main window background:");
            var (mainFontButton, mainFontPreview, mainFontLabel) = AddColorRow(colorsLayout, "Main window font color:");
            var (optionsColorButton, optionsColorPreview, optionsColorLabel) = AddColorRow(colorsLayout, "Options window background:");
            var (optionsFontButton, optionsFontPreview, optionsFontLabel) = AddColorRow(colorsLayout, "Options window font color:");
            colorsGroup.Controls.Add(colorsLayout);
            appearancePanel.Controls.Add(colorsGroup);

            var imagesGroup = CreateGroupBox("Skin bitmaps");
            var imagesLayout = CreateTableLayout();
            var mainBackground = AddFilePicker(imagesLayout, "Main background:");
            var mainRegion = AddFilePicker(imagesLayout, "Main region mask:");
            var optionsBackground = AddFilePicker(imagesLayout, "Options background:");
            var optionsRegion = AddFilePicker(imagesLayout, "Options region mask:");
            imagesGroup.Controls.Add(imagesLayout);
            appearancePanel.Controls.Add(imagesGroup);

            _closeButtonEditor = new SkinButtonEditor("Close button");
            _connectButtonEditor = new SkinButtonEditor("Connect button");
            _updateButtonEditor = new SkinButtonEditor("Update button");
            _optionsButtonEditor = new SkinButtonEditor("Options button");
            _secondaryCloseButtonEditor = new SkinButtonEditor("Secondary close button");
            _applyButtonEditor = new SkinButtonEditor("Apply button");

            buttonsPanel.Controls.Add(_closeButtonEditor);
            buttonsPanel.Controls.Add(_connectButtonEditor);
            buttonsPanel.Controls.Add(_updateButtonEditor);
            buttonsPanel.Controls.Add(_optionsButtonEditor);
            buttonsPanel.Controls.Add(_secondaryCloseButtonEditor);
            buttonsPanel.Controls.Add(_applyButtonEditor);

            var browserGroup = CreateGroupBox("Announcement browser");
            var browserLayout = CreateTableLayout();
            var browserLeft = AddNumericField(browserLayout, "Left:");
            var browserTop = AddNumericField(browserLayout, "Top:");
            var browserWidth = AddNumericField(browserLayout, "Width:");
            var browserHeight = AddNumericField(browserLayout, "Height:");
            browserGroup.Controls.Add(browserLayout);
            layoutPanel.Controls.Add(browserGroup);

            var statusGroup = CreateGroupBox("Server status panel");
            var statusLayout = CreateTableLayout();
            var statusLeft = AddNumericField(statusLayout, "Left:");
            var statusTop = AddNumericField(statusLayout, "Top:");
            var statusWidth = AddNumericField(statusLayout, "Width:");
            var statusHeight = AddNumericField(statusLayout, "Height:");
            statusGroup.Controls.Add(statusLayout);
            layoutPanel.Controls.Add(statusGroup);

            var idGroup = CreateGroupBox("Account section");
            var idLayout = CreateTableLayout();
            var idDescription = AddTextField(idLayout, "Description:");
            var idDescLeft = AddNumericField(idLayout, "Description left:");
            var idDescTop = AddNumericField(idLayout, "Description top:");
            var idDescWidth = AddNumericField(idLayout, "Description width:");
            var idDescHeight = AddNumericField(idLayout, "Description height:");
            var idInputLeft = AddNumericField(idLayout, "Input left:");
            var idInputTop = AddNumericField(idLayout, "Input top:");
            var idInputWidth = AddNumericField(idLayout, "Input width:");
            var idInputHeight = AddNumericField(idLayout, "Input height:");
            idGroup.Controls.Add(idLayout);
            layoutPanel.Controls.Add(idGroup);

            var resolutionGroup = CreateGroupBox("Resolution section");
            var resolutionLayout = CreateTableLayout();
            var resolutionDescription = AddTextField(resolutionLayout, "Description:");
            var resolutionDescLeft = AddNumericField(resolutionLayout, "Description left:");
            var resolutionDescTop = AddNumericField(resolutionLayout, "Description top:");
            var resolutionDescWidth = AddNumericField(resolutionLayout, "Description width:");
            var resolutionDescHeight = AddNumericField(resolutionLayout, "Description height:");
            var option640Left = AddNumericField(resolutionLayout, "640x480 left:");
            var option640Top = AddNumericField(resolutionLayout, "640x480 top:");
            var option800Left = AddNumericField(resolutionLayout, "800x600 left:");
            var option800Top = AddNumericField(resolutionLayout, "800x600 top:");
            var option1024Left = AddNumericField(resolutionLayout, "1024x768 left:");
            var option1024Top = AddNumericField(resolutionLayout, "1024x768 top:");
            var option1280Left = AddNumericField(resolutionLayout, "1280x1024 left:");
            var option1280Top = AddNumericField(resolutionLayout, "1280x1024 top:");
            resolutionGroup.Controls.Add(resolutionLayout);
            layoutPanel.Controls.Add(resolutionGroup);

            var soundGroup = CreateGroupBox("Sound section");
            var soundLayout = CreateTableLayout();
            var soundDescription = AddTextField(soundLayout, "Description:");
            var soundDescLeft = AddNumericField(soundLayout, "Description left:");
            var soundDescTop = AddNumericField(soundLayout, "Description top:");
            var soundDescWidth = AddNumericField(soundLayout, "Description width:");
            var soundDescHeight = AddNumericField(soundLayout, "Description height:");
            var soundOptionLeft = AddNumericField(soundLayout, "Sound option left:");
            var soundOptionTop = AddNumericField(soundLayout, "Sound option top:");
            var musicOptionLeft = AddNumericField(soundLayout, "Music option left:");
            var musicOptionTop = AddNumericField(soundLayout, "Music option top:");
            soundGroup.Controls.Add(soundLayout);
            layoutPanel.Controls.Add(soundGroup);

            RegisterCheckBinding(_useCustomSkinCheckBox, o => o.UseCustomSkin, (o, value) => o.UseCustomSkin = value, () => SetCustomSkinEnabled());

            RegisterColorBinding(mainColorButton, mainColorPreview, mainColorLabel, o => o.MainColor, (o, color) => o.MainColor = color);
            RegisterColorBinding(mainFontButton, mainFontPreview, mainFontLabel, o => o.MainFontColor, (o, color) => o.MainFontColor = color);
            RegisterColorBinding(optionsColorButton, optionsColorPreview, optionsColorLabel, o => o.OptionsColor, (o, color) => o.OptionsColor = color);
            RegisterColorBinding(optionsFontButton, optionsFontPreview, optionsFontLabel, o => o.OptionsFontColor, (o, color) => o.OptionsFontColor = color);

            RegisterFilePicker(mainBackground.TextBox, mainBackground.Button, o => o.MainBackgroundPath, (o, value) => o.MainBackgroundPath = value);
            RegisterFilePicker(mainRegion.TextBox, mainRegion.Button, o => o.MainRegionPath, (o, value) => o.MainRegionPath = value);
            RegisterFilePicker(optionsBackground.TextBox, optionsBackground.Button, o => o.OptionsBackgroundPath, (o, value) => o.OptionsBackgroundPath = value);
            RegisterFilePicker(optionsRegion.TextBox, optionsRegion.Button, o => o.OptionsRegionPath, (o, value) => o.OptionsRegionPath = value);

            RegisterNumericBinding(browserLeft, o => o.Browser.Left, (o, value) => o.Browser.Left = value);
            RegisterNumericBinding(browserTop, o => o.Browser.Top, (o, value) => o.Browser.Top = value);
            RegisterNumericBinding(browserWidth, o => o.Browser.Width, (o, value) => o.Browser.Width = value);
            RegisterNumericBinding(browserHeight, o => o.Browser.Height, (o, value) => o.Browser.Height = value);

            RegisterNumericBinding(statusLeft, o => o.ServerStatus.Left, (o, value) => o.ServerStatus.Left = value);
            RegisterNumericBinding(statusTop, o => o.ServerStatus.Top, (o, value) => o.ServerStatus.Top = value);
            RegisterNumericBinding(statusWidth, o => o.ServerStatus.Width, (o, value) => o.ServerStatus.Width = value);
            RegisterNumericBinding(statusHeight, o => o.ServerStatus.Height, (o, value) => o.ServerStatus.Height = value);

            RegisterTextBinding(idDescription, o => o.Id.Description, (o, value) => o.Id.Description = value);
            RegisterNumericBinding(idDescLeft, o => o.Id.DescriptionArea.Left, (o, value) => o.Id.DescriptionArea.Left = value);
            RegisterNumericBinding(idDescTop, o => o.Id.DescriptionArea.Top, (o, value) => o.Id.DescriptionArea.Top = value);
            RegisterNumericBinding(idDescWidth, o => o.Id.DescriptionArea.Width, (o, value) => o.Id.DescriptionArea.Width = value);
            RegisterNumericBinding(idDescHeight, o => o.Id.DescriptionArea.Height, (o, value) => o.Id.DescriptionArea.Height = value);
            RegisterNumericBinding(idInputLeft, o => o.Id.InputArea.Left, (o, value) => o.Id.InputArea.Left = value);
            RegisterNumericBinding(idInputTop, o => o.Id.InputArea.Top, (o, value) => o.Id.InputArea.Top = value);
            RegisterNumericBinding(idInputWidth, o => o.Id.InputArea.Width, (o, value) => o.Id.InputArea.Width = value);
            RegisterNumericBinding(idInputHeight, o => o.Id.InputArea.Height, (o, value) => o.Id.InputArea.Height = value);

            RegisterTextBinding(resolutionDescription, o => o.Resolution.Description, (o, value) => o.Resolution.Description = value);
            RegisterNumericBinding(resolutionDescLeft, o => o.Resolution.DescriptionArea.Left, (o, value) => o.Resolution.DescriptionArea.Left = value);
            RegisterNumericBinding(resolutionDescTop, o => o.Resolution.DescriptionArea.Top, (o, value) => o.Resolution.DescriptionArea.Top = value);
            RegisterNumericBinding(resolutionDescWidth, o => o.Resolution.DescriptionArea.Width, (o, value) => o.Resolution.DescriptionArea.Width = value);
            RegisterNumericBinding(resolutionDescHeight, o => o.Resolution.DescriptionArea.Height, (o, value) => o.Resolution.DescriptionArea.Height = value);
            RegisterNumericBinding(option640Left, o => o.Resolution.Option640x480.Left, (o, value) => o.Resolution.Option640x480.Left = value);
            RegisterNumericBinding(option640Top, o => o.Resolution.Option640x480.Top, (o, value) => o.Resolution.Option640x480.Top = value);
            RegisterNumericBinding(option800Left, o => o.Resolution.Option800x600.Left, (o, value) => o.Resolution.Option800x600.Left = value);
            RegisterNumericBinding(option800Top, o => o.Resolution.Option800x600.Top, (o, value) => o.Resolution.Option800x600.Top = value);
            RegisterNumericBinding(option1024Left, o => o.Resolution.Option1024x768.Left, (o, value) => o.Resolution.Option1024x768.Left = value);
            RegisterNumericBinding(option1024Top, o => o.Resolution.Option1024x768.Top, (o, value) => o.Resolution.Option1024x768.Top = value);
            RegisterNumericBinding(option1280Left, o => o.Resolution.Option1280x1024.Left, (o, value) => o.Resolution.Option1280x1024.Left = value);
            RegisterNumericBinding(option1280Top, o => o.Resolution.Option1280x1024.Top, (o, value) => o.Resolution.Option1280x1024.Top = value);

            RegisterTextBinding(soundDescription, o => o.Sound.Description, (o, value) => o.Sound.Description = value);
            RegisterNumericBinding(soundDescLeft, o => o.Sound.DescriptionArea.Left, (o, value) => o.Sound.DescriptionArea.Left = value);
            RegisterNumericBinding(soundDescTop, o => o.Sound.DescriptionArea.Top, (o, value) => o.Sound.DescriptionArea.Top = value);
            RegisterNumericBinding(soundDescWidth, o => o.Sound.DescriptionArea.Width, (o, value) => o.Sound.DescriptionArea.Width = value);
            RegisterNumericBinding(soundDescHeight, o => o.Sound.DescriptionArea.Height, (o, value) => o.Sound.DescriptionArea.Height = value);
            RegisterNumericBinding(soundOptionLeft, o => o.Sound.SoundOption.Left, (o, value) => o.Sound.SoundOption.Left = value);
            RegisterNumericBinding(soundOptionTop, o => o.Sound.SoundOption.Top, (o, value) => o.Sound.SoundOption.Top = value);
            RegisterNumericBinding(musicOptionLeft, o => o.Sound.MusicOption.Left, (o, value) => o.Sound.MusicOption.Left = value);
            RegisterNumericBinding(musicOptionTop, o => o.Sound.MusicOption.Top, (o, value) => o.Sound.MusicOption.Top = value);

            appearancePanel.Resize += (_, _) => ExpandGroupBoxes(appearancePanel);
            buttonsPanel.Resize += (_, _) => ExpandGroupBoxes(buttonsPanel);
            layoutPanel.Resize += (_, _) => ExpandGroupBoxes(layoutPanel);
        }

        public void Bind(SkinOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));

            _closeButtonEditor.Bind(options.CloseButton);
            _connectButtonEditor.Bind(options.ConnectButton);
            _updateButtonEditor.Bind(options.UpdateButton);
            _optionsButtonEditor.Bind(options.OptionsButton);
            _secondaryCloseButtonEditor.Bind(options.SecondaryCloseButton);
            _applyButtonEditor.Bind(options.ApplyButton);

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

                foreach (var binding in _numericBindings)
                {
                    var numeric = binding.Numeric;
                    var value = binding.Getter(_options);
                    var clamped = Math.Min((int)numeric.Maximum, Math.Max((int)numeric.Minimum, value));
                    numeric.Value = clamped;
                }

                foreach (var binding in _colorBindings)
                {
                    var color = binding.Getter(_options);
                    binding.Preview.BackColor = color;
                    binding.HexLabel.Text = ColorToText(color);
                }
            }
            finally
            {
                _updating = false;
            }

            _closeButtonEditor.RefreshValues();
            _connectButtonEditor.RefreshValues();
            _updateButtonEditor.RefreshValues();
            _optionsButtonEditor.RefreshValues();
            _secondaryCloseButtonEditor.RefreshValues();
            _applyButtonEditor.RefreshValues();

            SetCustomSkinEnabled();
        }

        private void RegisterCheckBinding(CheckBox checkBox, Func<SkinOptions, bool> getter, Action<SkinOptions, bool> setter, Action? changed = null)
        {
            checkBox.CheckedChanged += (_, _) =>
            {
                if (_options != null && !_updating)
                {
                    setter(_options, checkBox.Checked);
                    changed?.Invoke();
                }
            };
            _checkBindings.Add(new CheckBinding(checkBox, getter, setter));
        }

        private void RegisterTextBinding(TextBox textBox, Func<SkinOptions, string> getter, Action<SkinOptions, string> setter)
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

        private void RegisterNumericBinding(NumericUpDown numeric, Func<SkinOptions, int> getter, Action<SkinOptions, int> setter)
        {
            numeric.ValueChanged += (_, _) =>
            {
                if (_options != null && !_updating)
                {
                    setter(_options, (int)numeric.Value);
                }
            };
            _numericBindings.Add(new NumericBinding(numeric, getter, setter));
        }

        private void RegisterColorBinding(Button button, Panel preview, Label label, Func<SkinOptions, Color> getter, Action<SkinOptions, Color> setter)
        {
            button.Click += (_, _) =>
            {
                if (_options == null)
                {
                    return;
                }

                _colorDialog.Color = getter(_options);
                if (_colorDialog.ShowDialog(this) == DialogResult.OK)
                {
                    setter(_options, _colorDialog.Color);
                    preview.BackColor = _colorDialog.Color;
                    label.Text = ColorToText(_colorDialog.Color);
                }
            };
            _colorBindings.Add(new ColorBinding(button, preview, label, getter, setter));
        }

        private void RegisterFilePicker(TextBox textBox, Button button, Func<SkinOptions, string> getter, Action<SkinOptions, string> setter)
        {
            RegisterTextBinding(textBox, getter, setter);
            button.Click += (_, _) => BrowseForImage(textBox, setter);
            _filePickers.Add(new FilePicker(textBox, button));
        }

        private void BrowseForImage(TextBox target, Action<SkinOptions, string> setter)
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

        private void SetCustomSkinEnabled()
        {
            var enabled = _useCustomSkinCheckBox.Checked;
            foreach (var picker in _filePickers)
            {
                picker.TextBox.Enabled = enabled;
                picker.Button.Enabled = enabled;
            }

            _closeButtonEditor.SetImagePickersEnabled(enabled);
            _connectButtonEditor.SetImagePickersEnabled(enabled);
            _updateButtonEditor.SetImagePickersEnabled(enabled);
            _optionsButtonEditor.SetImagePickersEnabled(enabled);
            _secondaryCloseButtonEditor.SetImagePickersEnabled(enabled);
            _applyButtonEditor.SetImagePickersEnabled(enabled);
        }

        private static FlowLayoutPanel CreateFlowPanel()
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(6)
            };
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

        private static TableLayoutPanel CreateSingleColumnLayout()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            return layout;
        }

        private static TableLayoutPanel CreateColorTable()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            return layout;
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

        private static (Button Button, Panel Preview, Label Label) AddColorRow(TableLayoutPanel layout, string labelText)
        {
            var row = layout.RowCount;
            layout.RowCount++;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var label = new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left };
            layout.Controls.Add(label, 0, row);

            var preview = new Panel { Width = 40, Height = 18, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(6, 3, 6, 3) };
            layout.Controls.Add(preview, 1, row);

            var hexLabel = new Label { AutoSize = true, Anchor = AnchorStyles.Left };
            layout.Controls.Add(hexLabel, 2, row);

            var button = new Button { Text = "Change...", AutoSize = true };
            layout.Controls.Add(button, 3, row);

            return (button, preview, hexLabel);
        }

        private static TextBox AddTextField(TableLayoutPanel layout, string labelText)
        {
            var row = layout.RowCount;
            layout.RowCount++;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var label = new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left };
            layout.Controls.Add(label, 0, row);

            var textBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            layout.Controls.Add(textBox, 1, row);
            layout.SetColumnSpan(textBox, 2);

            return textBox;
        }

        private static NumericUpDown AddNumericField(TableLayoutPanel layout, string labelText)
        {
            var row = layout.RowCount;
            layout.RowCount++;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var label = new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left };
            layout.Controls.Add(label, 0, row);

            var numeric = new NumericUpDown
            {
                Minimum = -4096,
                Maximum = 4096,
                Increment = 1,
                Width = 120
            };
            layout.Controls.Add(numeric, 1, row);
            layout.SetColumnSpan(numeric, 2);

            return numeric;
        }

        private static (TextBox TextBox, Button Button) AddFilePicker(TableLayoutPanel layout, string labelText)
        {
            var row = layout.RowCount;
            layout.RowCount++;
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var label = new Label { Text = labelText, AutoSize = true, Anchor = AnchorStyles.Left };
            layout.Controls.Add(label, 0, row);

            var textBox = new TextBox { Anchor = AnchorStyles.Left | AnchorStyles.Right };
            layout.Controls.Add(textBox, 1, row);

            var button = new Button { Text = "Browse...", AutoSize = true };
            layout.Controls.Add(button, 2, row);

            return (textBox, button);
        }

        private static string ColorToText(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
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
                else if (control is SkinButtonEditor buttonEditor)
                {
                    buttonEditor.Width = Math.Max(100, width - buttonEditor.Margin.Horizontal);
                }
            }
        }
    }
}
