using System;
using System.IO;
using System.Windows.Forms;
using LauncherCS.Forms;

namespace LauncherCS
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var exePath = Application.ExecutablePath;
            var loader = new ExecutableDataReader();
            LauncherConfiguration? configuration = null;

            try
            {
                configuration = loader.LoadConfiguration(exePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load embedded configuration: {ex.Message}\nFalling back to Options.xml if available.",
                    "Launcher", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                var xmlPath = Path.Combine(AppContext.BaseDirectory, "Options.xml");
                if (File.Exists(xmlPath))
                {
                    using var fs = File.OpenRead(xmlPath);
                    configuration = ExecutableDataReader.LoadFromXml(fs);
                }
            }

            if (configuration == null)
            {
                MessageBox.Show("The launcher configuration could not be loaded.", "Launcher",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(configuration.Options.ServerName))
            {
                configuration.Options.ServerName = "MU Launcher";
            }

            if (string.IsNullOrWhiteSpace(configuration.Options.ServerPage))
            {
                configuration.Options.ServerPage = "SkyTeam";
            }

            Application.Run(new MainForm(configuration));
        }
    }
}
