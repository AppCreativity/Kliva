
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Kliva.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();

            //TODO: Glenn - We need this to change into Device Family state trigger, but doesn't seem to work?
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop")
            {
                this.LayoutRoot.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri(@"ms-appx:/Assets/Merckx_1920.jpg", UriKind.Absolute)) };
                this.ContentPanel.Margin = new Thickness() { Right = 0, Top = 0, Left = 0, Bottom = 100 };
                this.ContentPanel.HorizontalAlignment = HorizontalAlignment.Center;
            }

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
            {
                this.LayoutRoot.Background = new ImageBrush() { ImageSource = new BitmapImage(new Uri(@"ms-appx:/Assets/Brooks.jpg", UriKind.Absolute)) };
                this.ContentPanel.Margin = new Thickness() { Right = 10, Top = 0, Left = 10, Bottom = 10 };
                this.ContentPanel.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }
    }
}
