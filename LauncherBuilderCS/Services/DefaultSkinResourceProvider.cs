using System;
using System.Collections.Generic;

namespace LauncherBuilderCS.Services
{
    internal sealed class DefaultSkinResourceProvider
    {
        private readonly Dictionary<string, string> _resources;

        public DefaultSkinResourceProvider()
        {
            _resources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["bmain.bmp"] = DefaultSkinData.bmain,
                ["boptions.bmp"] = DefaultSkinData.boptions,
                ["cancel1.bmp"] = DefaultSkinData.cancel1,
                ["cancel2.bmp"] = DefaultSkinData.cancel2,
                ["connect1.bmp"] = DefaultSkinData.connect1,
                ["connect2.bmp"] = DefaultSkinData.connect2,
                ["update1.bmp"] = DefaultSkinData.update1,
                ["update2.bmp"] = DefaultSkinData.update2,
                ["option1.bmp"] = DefaultSkinData.option1,
                ["option2.bmp"] = DefaultSkinData.option2,
                ["apply1.bmp"] = DefaultSkinData.apply1,
                ["apply2.bmp"] = DefaultSkinData.apply2
            };
        }

        public string MainBackground => Get("bmain.bmp");

        public string OptionsBackground => Get("boptions.bmp");

        public string CloseNormal => Get("cancel1.bmp");

        public string CloseDown => Get("cancel2.bmp");

        public string ConnectNormal => Get("connect1.bmp");

        public string ConnectDown => Get("connect2.bmp");

        public string UpdateNormal => Get("update1.bmp");

        public string UpdateDown => Get("update2.bmp");

        public string OptionNormal => Get("option1.bmp");

        public string OptionDown => Get("option2.bmp");

        public string ApplyNormal => Get("apply1.bmp");

        public string ApplyDown => Get("apply2.bmp");

        private string Get(string fileName) => _resources[fileName];
    }
}
