using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Kliva.Models;
using Kliva.ViewModels;

namespace Kliva.Controls
{
    public sealed partial class AppInfoDialog : ContentDialog
    {
        public AppInfoDialog()
        {
            this.InitializeComponent();
            DataContext = ViewModelLocator.Get<SettingsViewModel>();
            ((SettingsViewModel)DataContext).ViewLoadedCommand.Execute(null);
        }

        private void OnContentDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }
    }
}
