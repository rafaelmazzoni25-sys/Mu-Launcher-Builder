using System.Drawing;
using System.Globalization;
using System.Security;
using System.Text;
using LauncherCS;

namespace LauncherBuilderCS.Services
{
    internal sealed class OptionXmlBuilder
    {
        private const string Header = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><Options>";
        private const string GuiFormat = "<GUI ShowBrowser=\"{0}\" ShowOption=\"{1}\" ShowUpdate=\"{2}\" ShowSplashScreen=\"{3}\" ShowServerStatus=\"{4}\"/>";
        private const string GeneralFormat = "<General BrowserURL=\"{0}\" BrowserLeft=\"{1}\" BrowserWidth=\"{2}\" BrowserTop=\"{3}\" BrowserHeight=\"{4}\" StatusLeft=\"{5}\" StatusWidth=\"{6}\" StatusTop=\"{7}\" StatusHeight=\"{8}\"/>";
        private const string SplashFormat = "<SplashScreen Data=\"{0}\" Region=\"{1}\"/>";
        private const string ServerFormat = "<Server ServerName=\"{0}\" ServerHostname=\"{1}\" ServerPort=\"{2}\" ServerPage=\"{3}\"/>";
        private const string UpdateFormat = "<Update UpdateData=\"{0}\"/>";
        private const string ColorsFormat = "<Colors MainColor=\"{0}\" MainFontColor=\"{1}\" OptionsColor=\"{2}\" OptionsFontColor=\"{3}\" />";
        private const string SkinFormat = "<Skin Background=\"{0}\" MainRegion=\"{1}\" Options=\"{2}\" OptionsRegion=\"{3}\" CL_N=\"{4}\" CL_D=\"{5}\" CL_X=\"{6}\" CL_Y=\"{7}\" CN_N=\"{8}\" CN_D=\"{9}\" CN_X=\"{10}\" CN_Y=\"{11}\" UP_N=\"{12}\" UP_D=\"{13}\" UP_X=\"{14}\" UP_Y=\"{15}\" OP_N=\"{16}\" OP_D=\"{17}\" OP_X=\"{18}\" OP_Y=\"{19}\" CL2_N=\"{20}\" CL2_D=\"{21}\" CL2_X=\"{22}\" CL2_Y=\"{23}\" AP_N=\"{24}\" AP_D=\"{25}\" AP_X=\"{26}\" AP_Y=\"{27}\" ID_TEXT=\"{28}\" ID_L=\"{29}\" ID_W=\"{30}\" ID_T=\"{31}\" ID_H=\"{32}\" IDB_L=\"{33}\" IDB_W=\"{34}\" IDB_T=\"{35}\" IDB_H=\"{36}\" RS_TEXT=\"{37}\" RS_L=\"{38}\" RS_W=\"{39}\" RS_T=\"{40}\" RS_H=\"{41}\" RS1_L=\"{42}\" RS1_T=\"{43}\" RS2_L=\"{44}\" RS2_T=\"{45}\" RS3_L=\"{46}\" RS3_T=\"{47}\" RS4_L=\"{48}\" RS4_T=\"{49}\" SN_TEXT=\"{50}\" SN_L=\"{51}\" SN_W=\"{52}\" SN_T=\"{53}\" SN_H=\"{54}\" SN1_L=\"{55}\" SN1_T=\"{56}\" SN2_L=\"{57}\" SN2_T=\"{58}\" />";
        private const string Footer = "</Options>";

        public string Build(LauncherConfiguration configuration)
        {
            var options = configuration.Options;
            var skin = configuration.Skin;

            var sb = new StringBuilder();
            sb.Append(Header);
            sb.AppendFormat(CultureInfo.InvariantCulture, GuiFormat,
                BoolToInt(options.ShowBrowse),
                BoolToInt(options.ShowOption),
                BoolToInt(options.ShowUpdate),
                BoolToInt(options.ShowSplash),
                BoolToInt(options.ShowStatus));

            sb.AppendFormat(CultureInfo.InvariantCulture, GeneralFormat,
                Escape(options.BrowseUrl),
                skin.Browser.Left,
                skin.Browser.Width,
                skin.Browser.Top,
                skin.Browser.Height,
                skin.ServerStatus.Left,
                skin.ServerStatus.Width,
                skin.ServerStatus.Top,
                skin.ServerStatus.Height);

            sb.AppendFormat(CultureInfo.InvariantCulture, SplashFormat,
                Escape(options.SplashData),
                Escape(options.SplashRegion));

            sb.AppendFormat(CultureInfo.InvariantCulture, ServerFormat,
                Escape(options.ServerName),
                Escape(options.ServerIp),
                Escape(options.ServerPort),
                Escape(options.ServerPage));

            sb.AppendFormat(CultureInfo.InvariantCulture, UpdateFormat, Escape(options.UpdateData));

            sb.AppendFormat(CultureInfo.InvariantCulture, ColorsFormat,
                ColorToString(options.MainColor),
                ColorToString(options.MainFontColor),
                ColorToString(options.OptionsColor),
                ColorToString(options.OptionsFontColor));

            sb.AppendFormat(CultureInfo.InvariantCulture, SkinFormat,
                Escape(skin.MainBackground),
                Escape(skin.MainRegion),
                Escape(skin.OptionBackground),
                Escape(skin.OptionRegion),
                Escape(skin.Buttons.Close.DataNormal),
                Escape(skin.Buttons.Close.DataDown),
                skin.Buttons.Close.X,
                skin.Buttons.Close.Y,
                Escape(skin.Buttons.Connect.DataNormal),
                Escape(skin.Buttons.Connect.DataDown),
                skin.Buttons.Connect.X,
                skin.Buttons.Connect.Y,
                Escape(skin.Buttons.Update.DataNormal),
                Escape(skin.Buttons.Update.DataDown),
                skin.Buttons.Update.X,
                skin.Buttons.Update.Y,
                Escape(skin.Buttons.Option.DataNormal),
                Escape(skin.Buttons.Option.DataDown),
                skin.Buttons.Option.X,
                skin.Buttons.Option.Y,
                Escape(skin.Buttons.Close2.DataNormal),
                Escape(skin.Buttons.Close2.DataDown),
                skin.Buttons.Close2.X,
                skin.Buttons.Close2.Y,
                Escape(skin.Buttons.Apply.DataNormal),
                Escape(skin.Buttons.Apply.DataDown),
                skin.Buttons.Apply.X,
                skin.Buttons.Apply.Y,
                Escape(skin.Id.Id),
                skin.Id.IdText.Left,
                skin.Id.IdText.Width,
                skin.Id.IdText.Top,
                skin.Id.IdText.Height,
                skin.Id.IdPos.Left,
                skin.Id.IdPos.Width,
                skin.Id.IdPos.Top,
                skin.Id.IdPos.Height,
                Escape(skin.Res.Resolution),
                skin.Res.ResolutionText.Left,
                skin.Res.ResolutionText.Width,
                skin.Res.ResolutionText.Top,
                skin.Res.ResolutionText.Height,
                skin.Res.ResOptions.Res1.Left,
                skin.Res.ResOptions.Res1.Top,
                skin.Res.ResOptions.Res2.Left,
                skin.Res.ResOptions.Res2.Top,
                skin.Res.ResOptions.Res3.Left,
                skin.Res.ResOptions.Res3.Top,
                skin.Res.ResOptions.Res4.Left,
                skin.Res.ResOptions.Res4.Top,
                Escape(skin.Sound.Sound),
                skin.Sound.SoundText.Left,
                skin.Sound.SoundText.Width,
                skin.Sound.SoundText.Top,
                skin.Sound.SoundText.Height,
                skin.Sound.SoundOptions.Sound.Left,
                skin.Sound.SoundOptions.Sound.Top,
                skin.Sound.SoundOptions.Music.Left,
                skin.Sound.SoundOptions.Music.Top);

            sb.Append(Footer);
            return sb.ToString();
        }

        private static int BoolToInt(bool value) => value ? 1 : 0;

        private static string Escape(string? value) => SecurityElement.Escape(value) ?? string.Empty;

        private static string ColorToString(Color color)
        {
            return ColorTranslator.ToWin32(color).ToString(CultureInfo.InvariantCulture);
        }
    }
}
