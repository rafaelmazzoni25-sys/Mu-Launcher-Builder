using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace LauncherCS.Forms
{
    public sealed class SplashForm : Form
    {
        private readonly LauncherConfiguration _configuration;
        private readonly Timer _timer;
        private Bitmap? _background;
        private Region? _region;

        public SplashForm(LauncherConfiguration configuration)
        {
            _configuration = configuration;

            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;

            _timer = new Timer { Interval = 3000 };
            _timer.Tick += (_, _) => Close();

            Load += (_, _) => InitializeSplash();
            Paint += OnPaint;
            FormClosed += (_, _) => CleanUp();
        }

        private void InitializeSplash()
        {
            _background?.Dispose();
            _region?.Dispose();
            _region = null;
            Region = null;

            if (!string.Equals(_configuration.Options.SplashData, "EMPTY", StringComparison.OrdinalIgnoreCase))
            {
                _background = ResourceDecoder.LoadBitmap(_configuration.Options.SplashData);
                ClientSize = _background.Size;
            }

            if (!string.Equals(_configuration.Options.SplashRegion, "EMPTY", StringComparison.OrdinalIgnoreCase))
            {
                using var regionBitmap = ResourceDecoder.LoadBitmap(_configuration.Options.SplashRegion);
                _region = ResourceDecoder.CreateRegion(regionBitmap);
                Region = _region;
            }

            _timer.Start();
        }

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            if (_background != null)
            {
                e.Graphics.DrawImageUnscaled(_background, Point.Empty);
            }
        }

        private void CleanUp()
        {
            _timer.Stop();
            _timer.Dispose();
            _background?.Dispose();
            _region?.Dispose();
            _region = null;
            Region = null;
        }
    }
}
