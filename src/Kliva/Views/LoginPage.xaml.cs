using System;
using Windows.UI.Xaml.Controls;

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
            Loaded += LoginPage_Loaded;
        }

        private void LoginPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var animation = blurBackground.Compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(0.0f, 0);
            animation.InsertKeyFrame(1.0f, 45);
            animation.Duration = TimeSpan.FromSeconds(10);

            blurBackground.VisualProperties.StartAnimation("BlurAmount", animation);
        }
    }
}
