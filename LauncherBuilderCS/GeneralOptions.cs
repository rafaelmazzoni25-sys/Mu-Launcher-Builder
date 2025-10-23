using System.ComponentModel;
using LauncherBuilderCS.Attributes;

namespace LauncherBuilderCS
{
    public sealed class GeneralOptions
    {
        public GeneralOptions()
        {
            ServerPort = "44405";
        }

        [Category("Features"), DisplayName("Show announcement panel"), Description("Displays the announcement browser on the main window.")]
        public bool ShowAnnouncement { get; set; }

        [Category("Features"), DisplayName("Show options window"), Description("Enables the options button and window.")]
        public bool ShowOptionsWindow { get; set; }

        [Category("Features"), DisplayName("Show update button"), Description("Enables the update button on the main window.")]
        public bool ShowUpdateButton { get; set; }

        [Category("Features"), DisplayName("Show splash screen"), Description("Displays a splash screen before the launcher.")]
        public bool ShowSplashScreen { get; set; }

        [Category("Features"), DisplayName("Show server status"), Description("Displays the server status panel on the main window.")]
        public bool ShowServerStatus { get; set; }

        [Category("Announcement"), DisplayName("Announcement URL"), Description("URL opened inside the announcement browser when enabled."), DefaultValue("")]
        public string AnnouncementUrl { get; set; } = string.Empty;

        [Category("Splash Screen"), DisplayName("Background image"), FilePath, Description("Bitmap displayed on the splash screen. Required when the splash screen is enabled."), DefaultValue("")]
        public string SplashBackgroundPath { get; set; } = string.Empty;

        [Category("Splash Screen"), DisplayName("Region mask"), FilePath, Description("Optional monochrome bitmap defining the splash screen region. Leave empty to disable."), DefaultValue("")]
        public string SplashRegionPath { get; set; } = string.Empty;

        [Category("Server"), DisplayName("Server name"), Description("Displayed name of the MU server."), DefaultValue("")]
        public string ServerName { get; set; } = string.Empty;

        [Category("Server"), DisplayName("Server page"), Description("Displayed website name or additional information."), DefaultValue("")]
        public string ServerPage { get; set; } = string.Empty;

        [Category("Server"), DisplayName("Server hostname"), Description("Hostname or IP address used by the launcher."), DefaultValue("")]
        public string ServerHost { get; set; } = string.Empty;

        [Category("Server"), DisplayName("Server port"), Description("Port used by the launcher when connecting."), DefaultValue("44405")]
        public string ServerPort { get; set; }

        [Category("Update"), DisplayName("Update data URL"), Description("Remote location of the update package index."), DefaultValue("")]
        public string UpdateUrl { get; set; } = string.Empty;

        internal string? ExistingSplashBackgroundData { get; set; }

        internal string? ExistingSplashRegionData { get; set; }
    }
}
