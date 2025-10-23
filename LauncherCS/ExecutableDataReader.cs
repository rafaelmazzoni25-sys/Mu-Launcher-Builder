using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace LauncherCS
{
    public sealed class ExecutableDataReader
    {
        public const int MagicNumber = 289792;

        public LauncherConfiguration LoadConfiguration(string executablePath)
        {
            using var exe = File.OpenRead(executablePath);
            if (exe.Length <= MagicNumber)
            {
                throw new InvalidOperationException("Executable does not contain embedded configuration data.");
            }

            exe.Position = MagicNumber;
            using var rawData = new MemoryStream();
            exe.CopyTo(rawData);
            rawData.Position = 0;

            ResourceDecoder.EncryptDecrypt(rawData);
            using var unpacked = ResourceDecoder.UnpackToMemory(rawData);
            return LoadFromXml(unpacked);
        }

        public static LauncherConfiguration LoadFromXml(Stream xmlStream)
        {
            var doc = XDocument.Load(xmlStream);
            var root = doc.Root ?? throw new InvalidOperationException("Options XML does not contain a root node.");

            var options = new LauncherOptions();
            var skin = new Skin();

            var gui = root.Element("GUI") ?? throw new InvalidOperationException("Missing GUI node.");
            options.ShowBrowse = ParseBool(gui.Attribute("ShowBrowser")?.Value);
            options.ShowOption = ParseBool(gui.Attribute("ShowOption")?.Value);
            options.ShowUpdate = ParseBool(gui.Attribute("ShowUpdate")?.Value);
            options.ShowSplash = ParseBool(gui.Attribute("ShowSplashScreen")?.Value);
            options.ShowStatus = ParseBool(gui.Attribute("ShowServerStatus")?.Value);

            var general = root.Element("General") ?? throw new InvalidOperationException("Missing General node.");
            options.BrowseUrl = general.Attribute("BrowserURL")?.Value ?? string.Empty;
            skin.Browser.Left = ParseInt(general.Attribute("BrowserLeft")?.Value);
            skin.Browser.Width = ParseInt(general.Attribute("BrowserWidth")?.Value);
            skin.Browser.Top = ParseInt(general.Attribute("BrowserTop")?.Value);
            skin.Browser.Height = ParseInt(general.Attribute("BrowserHeight")?.Value);
            skin.ServerStatus.Left = ParseInt(general.Attribute("StatusLeft")?.Value);
            skin.ServerStatus.Width = ParseInt(general.Attribute("StatusWidth")?.Value);
            skin.ServerStatus.Top = ParseInt(general.Attribute("StatusTop")?.Value);
            skin.ServerStatus.Height = ParseInt(general.Attribute("StatusHeight")?.Value);

            var splash = root.Element("SplashScreen") ?? throw new InvalidOperationException("Missing SplashScreen node.");
            options.SplashData = splash.Attribute("Data")?.Value ?? string.Empty;
            options.SplashRegion = splash.Attribute("Region")?.Value ?? string.Empty;

            var server = root.Element("Server") ?? throw new InvalidOperationException("Missing Server node.");
            options.ServerName = server.Attribute("ServerName")?.Value ?? string.Empty;
            options.ServerIp = server.Attribute("ServerHostname")?.Value ?? string.Empty;
            options.ServerPort = server.Attribute("ServerPort")?.Value ?? string.Empty;
            options.ServerPage = server.Attribute("ServerPage")?.Value ?? string.Empty;

            var update = root.Element("Update") ?? throw new InvalidOperationException("Missing Update node.");
            options.UpdateData = update.Attribute("UpdateData")?.Value ?? string.Empty;

            var colors = root.Element("Colors") ?? throw new InvalidOperationException("Missing Colors node.");
            options.MainColor = ParseColor(colors.Attribute("MainColor")?.Value);
            options.MainFontColor = ParseColor(colors.Attribute("MainFontColor")?.Value);
            options.OptionsColor = ParseColor(colors.Attribute("OptionsColor")?.Value);
            options.OptionsFontColor = ParseColor(colors.Attribute("OptionsFontColor")?.Value);

            var skinNode = root.Element("Skin") ?? throw new InvalidOperationException("Missing Skin node.");
            skin.MainBackground = skinNode.Attribute("Background")?.Value ?? string.Empty;
            skin.MainRegion = skinNode.Attribute("MainRegion")?.Value ?? string.Empty;
            skin.OptionBackground = skinNode.Attribute("Options")?.Value ?? string.Empty;
            skin.OptionRegion = skinNode.Attribute("OptionsRegion")?.Value ?? string.Empty;

            FillButton(skin.Buttons.Close, skinNode, "CL");
            FillButton(skin.Buttons.Connect, skinNode, "CN");
            FillButton(skin.Buttons.Update, skinNode, "UP");
            FillButton(skin.Buttons.Option, skinNode, "OP");
            FillButton(skin.Buttons.Close2, skinNode, "CL2");
            FillButton(skin.Buttons.Apply, skinNode, "AP");

            skin.Id.Id = skinNode.Attribute("ID_TEXT")?.Value ?? string.Empty;
            FillLabel(skin.Id.IdText, skinNode, "ID");
            FillLabel(skin.Id.IdPos, skinNode, "IDB");

            skin.Res.Resolution = skinNode.Attribute("RS_TEXT")?.Value ?? string.Empty;
            FillLabel(skin.Res.ResolutionText, skinNode, "RS");
            FillOption(skin.Res.ResOptions.Res1, skinNode, "RS1");
            FillOption(skin.Res.ResOptions.Res2, skinNode, "RS2");
            FillOption(skin.Res.ResOptions.Res3, skinNode, "RS3");
            FillOption(skin.Res.ResOptions.Res4, skinNode, "RS4");

            skin.Sound.Sound = skinNode.Attribute("SN_TEXT")?.Value ?? string.Empty;
            FillLabel(skin.Sound.SoundText, skinNode, "SN");
            FillOption(skin.Sound.SoundOptions.Sound, skinNode, "SN1");
            FillOption(skin.Sound.SoundOptions.Music, skinNode, "SN2");

            return new LauncherConfiguration(options, skin);
        }

        private static void FillButton(SkinButton button, XElement node, string prefix)
        {
            button.DataNormal = node.Attribute($"{prefix}_N")?.Value ?? string.Empty;
            button.DataDown = node.Attribute($"{prefix}_D")?.Value ?? string.Empty;
            button.X = ParseInt(node.Attribute($"{prefix}_X")?.Value);
            button.Y = ParseInt(node.Attribute($"{prefix}_Y")?.Value);
        }

        private static void FillLabel(LabelPosition label, XElement node, string prefix)
        {
            label.Left = ParseInt(node.Attribute($"{prefix}_L")?.Value);
            label.Width = ParseInt(node.Attribute($"{prefix}_W")?.Value);
            label.Top = ParseInt(node.Attribute($"{prefix}_T")?.Value);
            label.Height = ParseInt(node.Attribute($"{prefix}_H")?.Value);
        }

        private static void FillOption(OptionPosition option, XElement node, string prefix)
        {
            option.Left = ParseInt(node.Attribute($"{prefix}_L")?.Value);
            option.Top = ParseInt(node.Attribute($"{prefix}_T")?.Value);
        }

        private static bool ParseBool(string? value) => value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

        private static int ParseInt(string? value)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : 0;
        }

        private static Color ParseColor(string? value)
        {
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var raw))
            {
                return Color.Black;
            }

            return ColorTranslator.FromWin32(raw);
        }
    }
}
