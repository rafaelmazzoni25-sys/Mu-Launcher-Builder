using System;
using LauncherCS;

namespace LauncherBuilderCS.Services
{
    internal sealed class LauncherConfigurationBuilder
    {
        private readonly DefaultSkinResourceProvider _defaults;

        public LauncherConfigurationBuilder(DefaultSkinResourceProvider defaults)
        {
            _defaults = defaults;
        }

        public LauncherConfiguration Build(GeneralOptions generalOptions, SkinOptions skinOptions)
        {
            var options = new LauncherOptions
            {
                ShowBrowse = generalOptions.ShowAnnouncement,
                ShowOption = generalOptions.ShowOptionsWindow,
                ShowUpdate = generalOptions.ShowUpdateButton,
                ShowSplash = generalOptions.ShowSplashScreen,
                ShowStatus = generalOptions.ShowServerStatus,
                BrowseUrl = generalOptions.AnnouncementUrl ?? string.Empty,
                ServerName = generalOptions.ServerName ?? string.Empty,
                ServerPage = generalOptions.ServerPage ?? string.Empty,
                ServerIp = generalOptions.ServerHost ?? string.Empty,
                ServerPort = generalOptions.ServerPort ?? string.Empty,
                UpdateData = generalOptions.UpdateUrl ?? string.Empty,
                MainColor = skinOptions.MainColor,
                MainFontColor = skinOptions.MainFontColor,
                OptionsColor = skinOptions.OptionsColor,
                OptionsFontColor = skinOptions.OptionsFontColor
            };

            if (generalOptions.ShowSplashScreen)
            {
                options.SplashData = ResolveSplashImage(generalOptions.SplashBackgroundPath, generalOptions.ExistingSplashBackgroundData);
                options.SplashRegion = ResolveSplashRegion(generalOptions.SplashRegionPath, generalOptions.ExistingSplashRegionData);
            }
            else
            {
                options.SplashData = string.Empty;
                options.SplashRegion = "EMPTY";
            }

            var skin = SkinBuilder.BuildSkin(skinOptions, _defaults);
            return new LauncherConfiguration(options, skin);
        }

        private static string ResolveSplashImage(string path, string? existingData)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                return ResourceEncoder.CreateImageDataFromFile(path);
            }

            if (!string.IsNullOrEmpty(existingData))
            {
                return existingData;
            }

            throw new InvalidOperationException("The splash screen is enabled but no background image was provided.");
        }

        private static string ResolveSplashRegion(string path, string? existingData)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                return ResourceEncoder.CreateImageDataFromFile(path);
            }

            if (!string.IsNullOrEmpty(existingData))
            {
                return existingData;
            }

            return "EMPTY";
        }
    }
}
