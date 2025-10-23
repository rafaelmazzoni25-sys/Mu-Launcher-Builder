using System.Drawing;
using System.Windows.Forms;

namespace LauncherCS
{
    public sealed class ImageButton : Button
    {
        private Image? _normalImage;
        private Image? _downImage;

        public ImageButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.Transparent;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            TabStop = false;
        }

        public Image? NormalImage
        {
            get => _normalImage;
            set
            {
                _normalImage = value;
                BackgroundImage = value;
                BackgroundImageLayout = ImageLayout.Stretch;
                if (value != null)
                {
                    Size = value.Size;
                }
            }
        }

        public Image? DownImage
        {
            get => _downImage;
            set => _downImage = value;
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left && _downImage != null)
            {
                BackgroundImage = _downImage;
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            if (mevent.Button == MouseButtons.Left && _normalImage != null)
            {
                BackgroundImage = _normalImage;
            }
        }
    }
}
