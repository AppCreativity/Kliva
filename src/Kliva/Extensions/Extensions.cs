using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace Kliva.Extensions
{
    public static class Extensions
    {
        public static void AdjustSize(this ContentDialog dialog)
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

            if (size.Width > 1000.0)
            {
                dialog.MinWidth = 500;
                dialog.MinHeight = 250;
            }
            else
                dialog.MinWidth = dialog.MinHeight = 300;
        }
    }
}
