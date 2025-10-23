using System;
using System.IO;
using System.Windows.Forms;
using LauncherBuilderCS.Controls;
using LauncherBuilderCS.Services;
using LauncherCS;

namespace LauncherBuilderCS
{
    internal sealed class MainForm : Form
    {
        private readonly GeneralOptions _generalOptions = new();
        private readonly SkinOptions _skinOptions = new();
        private readonly DefaultSkinResourceProvider _defaultSkin = new();
        private readonly LauncherConfigurationBuilder _configurationBuilder;
        private readonly LauncherBuilderService _builderService = new();

        private readonly GeneralOptionsControl _generalOptionsControl;
        private readonly SkinOptionsControl _skinOptionsControl;
        private readonly TextBox _outputPathTextBox;
        private readonly Button _outputBrowseButton;
        private readonly Button _buildButton;
        private readonly UpdateBuilderControl _updateControl;

        private readonly ToolStrip _toolStrip;
        private readonly ToolStripButton _loadXmlButton;
        private readonly ToolStripButton _loadExecutableButton;
        private readonly ToolStripButton _saveXmlButton;

        private readonly OpenFileDialog _openXmlDialog = new() { Filter = "Options XML|*.xml|All files|*.*", RestoreDirectory = true };
        private readonly SaveFileDialog _saveXmlDialog = new() { Filter = "Options XML|*.xml|All files|*.*", FileName = "Options.xml", RestoreDirectory = true };
        private readonly OpenFileDialog _openExecutableDialog = new() { Filter = "Launcher executable|*.exe|All files|*.*", RestoreDirectory = true };
        private readonly SaveFileDialog _saveExecutableDialog = new() { Filter = "Launcher executable|*.exe|All files|*.*", FileName = "Launcher.exe", RestoreDirectory = true };

        public MainForm()
        {
            Text = "MU Launcher Builder (C#)";
            Width = 1000;
            Height = 720;

            _configurationBuilder = new LauncherConfigurationBuilder(_defaultSkin);

            _toolStrip = new ToolStrip
            {
                GripStyle = ToolStripGripStyle.Hidden,
                Dock = DockStyle.Top
            };

            _loadXmlButton = new ToolStripButton("Load XML") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            _loadXmlButton.Click += (_, _) => LoadOptionsFromXml();

            _loadExecutableButton = new ToolStripButton("Load Executable") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            _loadExecutableButton.Click += (_, _) => LoadOptionsFromExecutable();

            _saveXmlButton = new ToolStripButton("Save XML") { DisplayStyle = ToolStripItemDisplayStyle.Text };
            _saveXmlButton.Click += (_, _) => SaveOptionsToXml();

            _toolStrip.Items.AddRange(new ToolStripItem[] { _loadXmlButton, _loadExecutableButton, _saveXmlButton });
            Controls.Add(_toolStrip);

            var tabControl = new TabControl
            {
                Dock = DockStyle.Fill
            };

            var generalTab = new TabPage("General");
            var skinTab = new TabPage("Skin");
            var updateTab = new TabPage("Update Packages");

            tabControl.TabPages.Add(generalTab);
            tabControl.TabPages.Add(skinTab);
            tabControl.TabPages.Add(updateTab);

            Controls.Add(tabControl);

            _generalOptionsControl = new GeneralOptionsControl { Dock = DockStyle.Fill };
            _skinOptionsControl = new SkinOptionsControl { Dock = DockStyle.Fill };

            generalTab.Controls.Add(_generalOptionsControl);
            skinTab.Controls.Add(_skinOptionsControl);

            _updateControl = new UpdateBuilderControl();
            updateTab.Controls.Add(_updateControl);

            var bottomPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                ColumnCount = 3,
                Padding = new Padding(10),
                AutoSize = true
            };
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var outputLabel = new Label
            {
                Text = "Output executable:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            _outputPathTextBox = new TextBox
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            _outputBrowseButton = new Button
            {
                Text = "Browse...",
                AutoSize = true
            };
            _outputBrowseButton.Click += (_, _) => BrowseForOutputPath();

            _buildButton = new Button
            {
                Text = "Build Launcher",
                AutoSize = true
            };
            _buildButton.Click += (_, _) => BuildLauncher();

            bottomPanel.Controls.Add(outputLabel, 0, 0);
            bottomPanel.Controls.Add(_outputPathTextBox, 1, 0);
            bottomPanel.Controls.Add(_outputBrowseButton, 2, 0);
            bottomPanel.Controls.Add(_buildButton, 2, 1);

            Controls.Add(bottomPanel);

            _generalOptionsControl.Bind(_generalOptions);
            _skinOptionsControl.Bind(_skinOptions);

            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            var defaultOutput = Path.Combine(Environment.CurrentDirectory, "Launcher.exe");
            _outputPathTextBox.Text = defaultOutput;

            var defaultOptionsPath = Path.Combine(AppContext.BaseDirectory, "Options.xml");
            if (File.Exists(defaultOptionsPath))
            {
                try
                {
                    using var stream = File.OpenRead(defaultOptionsPath);
                    var configuration = ExecutableDataReader.LoadFromXml(stream);
                    ApplyConfiguration(configuration);
                }
                catch
                {
                    // Ignore failures and keep defaults.
                }
            }
            else
            {
                _generalOptionsControl.RefreshValues();
                _skinOptionsControl.RefreshValues();
            }
        }

        private void LoadOptionsFromXml()
        {
            if (_openXmlDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    using var stream = File.OpenRead(_openXmlDialog.FileName);
                    var configuration = ExecutableDataReader.LoadFromXml(stream);
                    ApplyConfiguration(configuration);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Failed to load the XML configuration: {ex.Message}", "Launcher Builder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadOptionsFromExecutable()
        {
            if (_openExecutableDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var reader = new ExecutableDataReader();
                    var configuration = reader.LoadConfiguration(_openExecutableDialog.FileName);
                    ApplyConfiguration(configuration);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Failed to read configuration from executable: {ex.Message}", "Launcher Builder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SaveOptionsToXml()
        {
            if (_saveXmlDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    var configuration = _configurationBuilder.Build(_generalOptions, _skinOptions);
                    _builderService.SaveOptionsXml(_saveXmlDialog.FileName, configuration);
                    MessageBox.Show(this, "Options saved successfully.", "Launcher Builder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Failed to save options: {ex.Message}", "Launcher Builder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ApplyConfiguration(LauncherConfiguration configuration)
        {
            var options = configuration.Options;
            var skin = configuration.Skin;

            _generalOptions.ShowAnnouncement = options.ShowBrowse;
            _generalOptions.ShowOptionsWindow = options.ShowOption;
            _generalOptions.ShowUpdateButton = options.ShowUpdate;
            _generalOptions.ShowSplashScreen = options.ShowSplash;
            _generalOptions.ShowServerStatus = options.ShowStatus;
            _generalOptions.AnnouncementUrl = options.BrowseUrl;
            _generalOptions.ServerName = options.ServerName;
            _generalOptions.ServerPage = options.ServerPage;
            _generalOptions.ServerHost = options.ServerIp;
            _generalOptions.ServerPort = options.ServerPort;
            _generalOptions.UpdateUrl = options.UpdateData;
            _generalOptions.SplashBackgroundPath = string.Empty;
            _generalOptions.SplashRegionPath = string.Empty;
            _generalOptions.ExistingSplashBackgroundData = options.SplashData;
            _generalOptions.ExistingSplashRegionData = options.SplashRegion;

            _skinOptions.MainColor = options.MainColor;
            _skinOptions.MainFontColor = options.MainFontColor;
            _skinOptions.OptionsColor = options.OptionsColor;
            _skinOptions.OptionsFontColor = options.OptionsFontColor;

            _skinOptions.Browser.Left = skin.Browser.Left;
            _skinOptions.Browser.Top = skin.Browser.Top;
            _skinOptions.Browser.Width = skin.Browser.Width;
            _skinOptions.Browser.Height = skin.Browser.Height;

            _skinOptions.ServerStatus.Left = skin.ServerStatus.Left;
            _skinOptions.ServerStatus.Top = skin.ServerStatus.Top;
            _skinOptions.ServerStatus.Width = skin.ServerStatus.Width;
            _skinOptions.ServerStatus.Height = skin.ServerStatus.Height;

            _skinOptions.Id.Description = skin.Id.Id;
            CopyLabel(_skinOptions.Id.DescriptionArea, skin.Id.IdText);
            CopyLabel(_skinOptions.Id.InputArea, skin.Id.IdPos);

            _skinOptions.Resolution.Description = skin.Res.Resolution;
            CopyLabel(_skinOptions.Resolution.DescriptionArea, skin.Res.ResolutionText);
            CopyOption(_skinOptions.Resolution.Option640x480, skin.Res.ResOptions.Res1);
            CopyOption(_skinOptions.Resolution.Option800x600, skin.Res.ResOptions.Res2);
            CopyOption(_skinOptions.Resolution.Option1024x768, skin.Res.ResOptions.Res3);
            CopyOption(_skinOptions.Resolution.Option1280x1024, skin.Res.ResOptions.Res4);

            _skinOptions.Sound.Description = skin.Sound.Sound;
            CopyLabel(_skinOptions.Sound.DescriptionArea, skin.Sound.SoundText);
            CopyOption(_skinOptions.Sound.SoundOption, skin.Sound.SoundOptions.Sound);
            CopyOption(_skinOptions.Sound.MusicOption, skin.Sound.SoundOptions.Music);

            CopyButton(_skinOptions.CloseButton, skin.Buttons.Close);
            CopyButton(_skinOptions.ConnectButton, skin.Buttons.Connect);
            CopyButton(_skinOptions.UpdateButton, skin.Buttons.Update);
            CopyButton(_skinOptions.OptionsButton, skin.Buttons.Option);
            CopyButton(_skinOptions.SecondaryCloseButton, skin.Buttons.Close2);
            CopyButton(_skinOptions.ApplyButton, skin.Buttons.Apply);

            _skinOptions.ExistingMainBackgroundData = skin.MainBackground;
            _skinOptions.ExistingMainRegionData = skin.MainRegion;
            _skinOptions.ExistingOptionsBackgroundData = skin.OptionBackground;
            _skinOptions.ExistingOptionsRegionData = skin.OptionRegion;
            _skinOptions.MainBackgroundPath = string.Empty;
            _skinOptions.MainRegionPath = string.Empty;
            _skinOptions.OptionsBackgroundPath = string.Empty;
            _skinOptions.OptionsRegionPath = string.Empty;

            _skinOptions.UseCustomSkin = !IsDefaultSkin(skin);

            _generalOptionsControl.RefreshValues();
            _skinOptionsControl.RefreshValues();
        }

        private static void CopyLabel(LabelLayout target, LabelPosition source)
        {
            target.Left = source.Left;
            target.Top = source.Top;
            target.Width = source.Width;
            target.Height = source.Height;
        }

        private static void CopyOption(OptionPoint target, OptionPosition source)
        {
            target.Left = source.Left;
            target.Top = source.Top;
        }

        private static void CopyButton(SkinButtonAsset target, SkinButton source)
        {
            target.X = source.X;
            target.Y = source.Y;
            target.NormalPath = string.Empty;
            target.DownPath = string.Empty;
            target.ExistingNormalData = source.DataNormal;
            target.ExistingDownData = source.DataDown;
        }

        private bool IsDefaultSkin(Skin skin)
        {
            return string.Equals(skin.MainBackground, _defaultSkin.MainBackground, StringComparison.Ordinal) &&
                   string.Equals(skin.OptionBackground, _defaultSkin.OptionsBackground, StringComparison.Ordinal) &&
                   string.Equals(skin.MainRegion, "EMPTY", StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(skin.OptionRegion, "EMPTY", StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(skin.Buttons.Close.DataNormal, _defaultSkin.CloseNormal, StringComparison.Ordinal) &&
                   string.Equals(skin.Buttons.Close.DataDown, _defaultSkin.CloseDown, StringComparison.Ordinal) &&
                   string.Equals(skin.Buttons.Connect.DataNormal, _defaultSkin.ConnectNormal, StringComparison.Ordinal) &&
                   string.Equals(skin.Buttons.Connect.DataDown, _defaultSkin.ConnectDown, StringComparison.Ordinal) &&
                   string.Equals(skin.Buttons.Update.DataNormal, _defaultSkin.UpdateNormal, StringComparison.Ordinal) &&
                   string.Equals(skin.Buttons.Update.DataDown, _defaultSkin.UpdateDown, StringComparison.Ordinal) &&
                   string.Equals(skin.Buttons.Option.DataNormal, _defaultSkin.OptionNormal, StringComparison.Ordinal) &&
                   string.Equals(skin.Buttons.Option.DataDown, _defaultSkin.OptionDown, StringComparison.Ordinal) &&
                   string.Equals(skin.Buttons.Close2.DataNormal, _defaultSkin.CloseNormal, StringComparison.Ordinal) &&
                   string.Equals(skin.Buttons.Close2.DataDown, _defaultSkin.CloseDown, StringComparison.Ordinal) &&
                   string.Equals(skin.Buttons.Apply.DataNormal, _defaultSkin.ApplyNormal, StringComparison.Ordinal) &&
                   string.Equals(skin.Buttons.Apply.DataDown, _defaultSkin.ApplyDown, StringComparison.Ordinal);
        }

        private void BrowseForOutputPath()
        {
            if (_saveExecutableDialog.ShowDialog(this) == DialogResult.OK)
            {
                _outputPathTextBox.Text = _saveExecutableDialog.FileName;
            }
        }

        private void BuildLauncher()
        {
            try
            {
                var configuration = _configurationBuilder.Build(_generalOptions, _skinOptions);
                var output = _outputPathTextBox.Text;
                if (string.IsNullOrWhiteSpace(output))
                {
                    if (_saveExecutableDialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    output = _saveExecutableDialog.FileName;
                    _outputPathTextBox.Text = output;
                }

                _builderService.BuildExecutable(output, configuration);
                MessageBox.Show(this, "Launcher executable created successfully.", "Launcher Builder", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to build the launcher: {ex.Message}", "Launcher Builder", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
