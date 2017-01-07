using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kliva.Controls
{
    public sealed partial class AppInfoDialog : ContentDialog
    {
        public AppInfoDialog()
        {
            this.InitializeComponent(); 
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
