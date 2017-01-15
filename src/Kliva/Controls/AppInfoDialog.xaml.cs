using Windows.UI.Xaml.Controls;
using Kliva.ViewModels;

namespace Kliva.Controls
{
    public sealed partial class AppInfoDialog : ContentDialog
    {
        public AppInfoDialog()
        {
            InitializeComponent();
            DataContext = ViewModelLocator.Get<SettingsViewModel>();
            ((SettingsViewModel)DataContext).ViewLoadedCommand.Execute(null);
        }

        private void OnContentDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Hide();
        }
    }
}
