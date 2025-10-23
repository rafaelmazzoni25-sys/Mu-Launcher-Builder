using System.Drawing;

namespace LauncherCS
{
    public sealed class LauncherConfiguration
    {
        public LauncherOptions Options { get; }
        public Skin Skin { get; }

        public LauncherConfiguration(LauncherOptions options, Skin skin)
        {
            Options = options;
            Skin = skin;
        }
    }

    public sealed class LauncherOptions
    {
        public bool ShowBrowse { get; set; }
        public bool ShowOption { get; set; }
        public bool ShowUpdate { get; set; }
        public bool ShowSplash { get; set; }
        public bool ShowStatus { get; set; }
        public Color MainColor { get; set; }
        public Color MainFontColor { get; set; }
        public Color OptionsColor { get; set; }
        public Color OptionsFontColor { get; set; }
        public string BrowseUrl { get; set; } = string.Empty;
        public string SplashData { get; set; } = string.Empty;
        public string SplashRegion { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public string ServerIp { get; set; } = string.Empty;
        public string ServerPort { get; set; } = string.Empty;
        public string ServerPage { get; set; } = string.Empty;
        public string UpdateData { get; set; } = string.Empty;
    }

    public sealed class Skin
    {
        public SkinButtons Buttons { get; } = new();
        public string MainBackground { get; set; } = string.Empty;
        public string MainRegion { get; set; } = string.Empty;
        public SkinBrowser Browser { get; } = new();
        public LabelPosition ServerStatus { get; } = new();
        public string OptionBackground { get; set; } = string.Empty;
        public string OptionRegion { get; set; } = string.Empty;
        public IdOption Id { get; } = new();
        public ResolutionOption Res { get; } = new();
        public SoundOption Sound { get; } = new();
    }

    public sealed class SkinButtons
    {
        public SkinButton Close { get; } = new();
        public SkinButton Connect { get; } = new();
        public SkinButton Update { get; } = new();
        public SkinButton Option { get; } = new();
        public SkinButton Close2 { get; } = new();
        public SkinButton Apply { get; } = new();
    }

    public sealed class SkinButton
    {
        public string DataNormal { get; set; } = string.Empty;
        public string DataDown { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
    }

    public sealed class SkinBrowser
    {
        public int Left { get; set; }
        public int Width { get; set; }
        public int Top { get; set; }
        public int Height { get; set; }
    }

    public sealed class LabelPosition
    {
        public int Left { get; set; }
        public int Width { get; set; }
        public int Top { get; set; }
        public int Height { get; set; }
    }

    public sealed class OptionPosition
    {
        public int Left { get; set; }
        public int Top { get; set; }
    }

    public sealed class ResolutionOptions
    {
        public OptionPosition Res1 { get; } = new();
        public OptionPosition Res2 { get; } = new();
        public OptionPosition Res3 { get; } = new();
        public OptionPosition Res4 { get; } = new();
    }

    public sealed class SoundOptions
    {
        public OptionPosition Sound { get; } = new();
        public OptionPosition Music { get; } = new();
    }

    public sealed class IdOption
    {
        public string Id { get; set; } = string.Empty;
        public LabelPosition IdText { get; } = new();
        public LabelPosition IdPos { get; } = new();
    }

    public sealed class ResolutionOption
    {
        public string Resolution { get; set; } = string.Empty;
        public LabelPosition ResolutionText { get; } = new();
        public ResolutionOptions ResOptions { get; } = new();
    }

    public sealed class SoundOption
    {
        public string Sound { get; set; } = string.Empty;
        public LabelPosition SoundText { get; } = new();
        public SoundOptions SoundOptions { get; } = new();
    }
}
