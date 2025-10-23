using System;
using LauncherCS;

namespace LauncherBuilderCS.Services
{
    internal static class SkinBuilder
    {
        public static Skin BuildSkin(SkinOptions options, DefaultSkinResourceProvider defaults)
        {
            var skin = new Skin();

            if (options.UseCustomSkin)
            {
                skin.MainBackground = ResolveImage(options.MainBackgroundPath, options.ExistingMainBackgroundData, allowFallback: false);
                skin.MainRegion = ResolveRegion(options.MainRegionPath, options.ExistingMainRegionData);
                skin.OptionBackground = ResolveImage(options.OptionsBackgroundPath, options.ExistingOptionsBackgroundData, allowFallback: false);
                skin.OptionRegion = ResolveRegion(options.OptionsRegionPath, options.ExistingOptionsRegionData);

                ApplyButtonData(skin.Buttons.Close, options.CloseButton, defaults.CloseNormal, defaults.CloseDown, requireFiles: true);
                ApplyButtonData(skin.Buttons.Connect, options.ConnectButton, defaults.ConnectNormal, defaults.ConnectDown, requireFiles: true);
                ApplyButtonData(skin.Buttons.Update, options.UpdateButton, defaults.UpdateNormal, defaults.UpdateDown, requireFiles: true);
                ApplyButtonData(skin.Buttons.Option, options.OptionsButton, defaults.OptionNormal, defaults.OptionDown, requireFiles: true);
                ApplyButtonData(skin.Buttons.Close2, options.SecondaryCloseButton, defaults.CloseNormal, defaults.CloseDown, requireFiles: true);
                ApplyButtonData(skin.Buttons.Apply, options.ApplyButton, defaults.ApplyNormal, defaults.ApplyDown, requireFiles: true);
            }
            else
            {
                skin.MainBackground = defaults.MainBackground;
                skin.MainRegion = "EMPTY";
                skin.OptionBackground = defaults.OptionsBackground;
                skin.OptionRegion = "EMPTY";

                ApplyButtonData(skin.Buttons.Close, options.CloseButton, defaults.CloseNormal, defaults.CloseDown, requireFiles: false, preferFallback: true);
                ApplyButtonData(skin.Buttons.Connect, options.ConnectButton, defaults.ConnectNormal, defaults.ConnectDown, requireFiles: false, preferFallback: true);
                ApplyButtonData(skin.Buttons.Update, options.UpdateButton, defaults.UpdateNormal, defaults.UpdateDown, requireFiles: false, preferFallback: true);
                ApplyButtonData(skin.Buttons.Option, options.OptionsButton, defaults.OptionNormal, defaults.OptionDown, requireFiles: false, preferFallback: true);
                ApplyButtonData(skin.Buttons.Close2, options.SecondaryCloseButton, defaults.CloseNormal, defaults.CloseDown, requireFiles: false, preferFallback: true);
                ApplyButtonData(skin.Buttons.Apply, options.ApplyButton, defaults.ApplyNormal, defaults.ApplyDown, requireFiles: false, preferFallback: true);
            }

            skin.Browser.Left = options.Browser.Left;
            skin.Browser.Width = options.Browser.Width;
            skin.Browser.Top = options.Browser.Top;
            skin.Browser.Height = options.Browser.Height;

            skin.ServerStatus.Left = options.ServerStatus.Left;
            skin.ServerStatus.Width = options.ServerStatus.Width;
            skin.ServerStatus.Top = options.ServerStatus.Top;
            skin.ServerStatus.Height = options.ServerStatus.Height;

            skin.Id.Id = options.Id.Description;
            ApplyLabelLayout(skin.Id.IdText, options.Id.DescriptionArea);
            ApplyLabelLayout(skin.Id.IdPos, options.Id.InputArea);

            skin.Res.Resolution = options.Resolution.Description;
            ApplyLabelLayout(skin.Res.ResolutionText, options.Resolution.DescriptionArea);
            ApplyOption(skin.Res.ResOptions.Res1, options.Resolution.Option640x480);
            ApplyOption(skin.Res.ResOptions.Res2, options.Resolution.Option800x600);
            ApplyOption(skin.Res.ResOptions.Res3, options.Resolution.Option1024x768);
            ApplyOption(skin.Res.ResOptions.Res4, options.Resolution.Option1280x1024);

            skin.Sound.Sound = options.Sound.Description;
            ApplyLabelLayout(skin.Sound.SoundText, options.Sound.DescriptionArea);
            ApplyOption(skin.Sound.SoundOptions.Sound, options.Sound.SoundOption);
            ApplyOption(skin.Sound.SoundOptions.Music, options.Sound.MusicOption);

            return skin;
        }

        private static void ApplyLabelLayout(LabelPosition target, LabelLayout source)
        {
            target.Left = source.Left;
            target.Width = source.Width;
            target.Top = source.Top;
            target.Height = source.Height;
        }

        private static void ApplyOption(OptionPosition target, OptionPoint source)
        {
            target.Left = source.Left;
            target.Top = source.Top;
        }

        private static string ResolveImage(string path, string? existing, bool allowFallback, string? fallback = null)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                return ResourceEncoder.CreateImageDataFromFile(path);
            }

            if (!string.IsNullOrEmpty(existing))
            {
                return existing;
            }

            if (allowFallback && !string.IsNullOrEmpty(fallback))
            {
                return fallback;
            }

            throw new InvalidOperationException("Missing required image data. Please specify an image file or load an existing configuration.");
        }

        private static string ResolveRegion(string path, string? existing)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                return ResourceEncoder.CreateImageDataFromFile(path);
            }

            if (!string.IsNullOrEmpty(existing))
            {
                return existing;
            }

            return "EMPTY";
        }

        private static void ApplyButtonData(SkinButton button, SkinButtonAsset asset, string fallbackNormal, string fallbackDown, bool requireFiles, bool preferFallback = false)
        {
            button.X = asset.X;
            button.Y = asset.Y;

            button.DataNormal = ResolveButtonImage(asset.NormalPath, asset.ExistingNormalData, fallbackNormal, requireFiles, preferFallback);
            button.DataDown = ResolveButtonImage(asset.DownPath, asset.ExistingDownData, fallbackDown, requireFiles, preferFallback);
        }

        private static string ResolveButtonImage(string path, string? existing, string fallback, bool requireFiles, bool preferFallback)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                return ResourceEncoder.CreateImageDataFromFile(path);
            }

            if (!string.IsNullOrEmpty(existing) && !preferFallback)
            {
                return existing;
            }

            if (!requireFiles)
            {
                return fallback;
            }

            throw new InvalidOperationException("Missing required button image. Please provide a file path or load an existing configuration.");
        }
    }
}
