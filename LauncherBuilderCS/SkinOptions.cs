using System.ComponentModel;
using System.Drawing;
using LauncherBuilderCS.Attributes;

namespace LauncherBuilderCS
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class SkinOptions
    {
        public SkinOptions()
        {
            MainColor = ColorTranslator.FromWin32(0);
            MainFontColor = ColorTranslator.FromWin32(16777215);
            OptionsColor = ColorTranslator.FromWin32(0);
            OptionsFontColor = ColorTranslator.FromWin32(16777215);

            Browser = new BrowserLayout(8, 432, 33, 324);
            ServerStatus = new LabelLayout(8, 425, 360, 8);

            Id = new IdSection
            {
                Description = "Enter your account ID to have it automatically entered upon login.",
                DescriptionArea = new LabelLayout(16, 417, 40, 41),
                InputArea = new LabelLayout(160, 121, 88, 21)
            };

            Resolution = new ResolutionSection
            {
                Description = "Select the resolution rate. Higher resolutions may enable clearer graphics, but may require higher system specifications.",
                DescriptionArea = new LabelLayout(16, 417, 152, 41),
                Option640x480 = new OptionPoint(16, 200),
                Option800x600 = new OptionPoint(120, 200),
                Option1024x768 = new OptionPoint(224, 200),
                Option1280x1024 = new OptionPoint(344, 200)
            };

            Sound = new SoundSection
            {
                Description = "Select to enable or disable sound. Selecting option will enable sound.",
                DescriptionArea = new LabelLayout(16, 417, 264, 41),
                SoundOption = new OptionPoint(56, 312),
                MusicOption = new OptionPoint(224, 312)
            };

            CloseButton = new SkinButtonAsset(306, 368);
            ConnectButton = new SkinButtonAsset(219, 368);
            UpdateButton = new SkinButtonAsset(132, 368);
            OptionsButton = new SkinButtonAsset(45, 368);
            SecondaryCloseButton = new SkinButtonAsset(219, 368);
            ApplyButton = new SkinButtonAsset(132, 368);
        }

        [Category("General"), DisplayName("Use custom skin"), Description("Enable to provide custom images and layout information.")]
        public bool UseCustomSkin { get; set; }

        [Category("Colors"), DisplayName("Main window background color")]
        public Color MainColor { get; set; }

        [Category("Colors"), DisplayName("Main window font color")]
        public Color MainFontColor { get; set; }

        [Category("Colors"), DisplayName("Options window background color")]
        public Color OptionsColor { get; set; }

        [Category("Colors"), DisplayName("Options window font color")]
        public Color OptionsFontColor { get; set; }

        [Category("Images"), DisplayName("Main background"), FilePath]
        public string MainBackgroundPath { get; set; } = string.Empty;

        [Category("Images"), DisplayName("Main region mask"), FilePath]
        public string MainRegionPath { get; set; } = string.Empty;

        [Category("Images"), DisplayName("Options background"), FilePath]
        public string OptionsBackgroundPath { get; set; } = string.Empty;

        [Category("Images"), DisplayName("Options region mask"), FilePath]
        public string OptionsRegionPath { get; set; } = string.Empty;

        [Category("Buttons"), DisplayName("Close button")]
        public SkinButtonAsset CloseButton { get; }

        [Category("Buttons"), DisplayName("Connect button")]
        public SkinButtonAsset ConnectButton { get; }

        [Category("Buttons"), DisplayName("Update button")]
        public SkinButtonAsset UpdateButton { get; }

        [Category("Buttons"), DisplayName("Options button")]
        public SkinButtonAsset OptionsButton { get; }

        [Category("Buttons"), DisplayName("Secondary close button")]
        public SkinButtonAsset SecondaryCloseButton { get; }

        [Category("Buttons"), DisplayName("Apply button")]
        public SkinButtonAsset ApplyButton { get; }

        [Category("Layout"), DisplayName("Browser area")]
        public BrowserLayout Browser { get; }

        [Category("Layout"), DisplayName("Server status area")]
        public LabelLayout ServerStatus { get; }

        [Category("Texts"), DisplayName("Account section")]
        public IdSection Id { get; }

        [Category("Texts"), DisplayName("Resolution section")]
        public ResolutionSection Resolution { get; }

        [Category("Texts"), DisplayName("Sound section")]
        public SoundSection Sound { get; }

        internal string? ExistingMainBackgroundData { get; set; }
        internal string? ExistingMainRegionData { get; set; }
        internal string? ExistingOptionsBackgroundData { get; set; }
        internal string? ExistingOptionsRegionData { get; set; }

        public override string ToString() => UseCustomSkin ? "Custom skin" : "Default skin";
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class SkinButtonAsset
    {
        public SkinButtonAsset(int x, int y)
        {
            X = x;
            Y = y;
        }

        [Category("Images"), DisplayName("Normal state"), FilePath]
        public string NormalPath { get; set; } = string.Empty;

        [Category("Images"), DisplayName("Pressed state"), FilePath]
        public string DownPath { get; set; } = string.Empty;

        [Category("Layout"), DisplayName("X position")]
        public int X { get; set; }

        [Category("Layout"), DisplayName("Y position")]
        public int Y { get; set; }

        internal string? ExistingNormalData { get; set; }

        internal string? ExistingDownData { get; set; }

        public override string ToString() => $"{X}, {Y}";
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class BrowserLayout
    {
        public BrowserLayout(int left, int width, int top, int height)
        {
            Left = left;
            Width = width;
            Top = top;
            Height = height;
        }

        [DisplayName("Left")]
        public int Left { get; set; }

        [DisplayName("Width")]
        public int Width { get; set; }

        [DisplayName("Top")]
        public int Top { get; set; }

        [DisplayName("Height")]
        public int Height { get; set; }

        public override string ToString() => $"{Left}, {Top}, {Width}x{Height}";
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class LabelLayout
    {
        public LabelLayout()
        {
        }

        public LabelLayout(int left, int width, int top, int height)
        {
            Left = left;
            Width = width;
            Top = top;
            Height = height;
        }

        [DisplayName("Left")]
        public int Left { get; set; }

        [DisplayName("Width")]
        public int Width { get; set; }

        [DisplayName("Top")]
        public int Top { get; set; }

        [DisplayName("Height")]
        public int Height { get; set; }

        public override string ToString() => $"{Left}, {Top}, {Width}x{Height}";
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class OptionPoint
    {
        public OptionPoint()
        {
        }

        public OptionPoint(int left, int top)
        {
            Left = left;
            Top = top;
        }

        [DisplayName("Left")]
        public int Left { get; set; }

        [DisplayName("Top")]
        public int Top { get; set; }

        public override string ToString() => $"{Left}, {Top}";
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class IdSection
    {
        [Category("Content"), DisplayName("Description"), DefaultValue("")]
        public string Description { get; set; } = string.Empty;

        [Category("Layout"), DisplayName("Description area")]
        public LabelLayout DescriptionArea { get; set; } = new();

        [Category("Layout"), DisplayName("Input area")]
        public LabelLayout InputArea { get; set; } = new();

        public override string ToString() => Description;
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ResolutionSection
    {
        [Category("Content"), DisplayName("Description"), DefaultValue("")]
        public string Description { get; set; } = string.Empty;

        [Category("Layout"), DisplayName("Description area")]
        public LabelLayout DescriptionArea { get; set; } = new();

        [Category("Options"), DisplayName("640x480 option")]
        public OptionPoint Option640x480 { get; set; } = new();

        [Category("Options"), DisplayName("800x600 option")]
        public OptionPoint Option800x600 { get; set; } = new();

        [Category("Options"), DisplayName("1024x768 option")]
        public OptionPoint Option1024x768 { get; set; } = new();

        [Category("Options"), DisplayName("1280x1024 option")]
        public OptionPoint Option1280x1024 { get; set; } = new();

        public override string ToString() => Description;
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class SoundSection
    {
        [Category("Content"), DisplayName("Description"), DefaultValue("")]
        public string Description { get; set; } = string.Empty;

        [Category("Layout"), DisplayName("Description area")]
        public LabelLayout DescriptionArea { get; set; } = new();

        [Category("Options"), DisplayName("Sound toggle position")]
        public OptionPoint SoundOption { get; set; } = new();

        [Category("Options"), DisplayName("Music toggle position")]
        public OptionPoint MusicOption { get; set; } = new();

        public override string ToString() => Description;
    }
}
