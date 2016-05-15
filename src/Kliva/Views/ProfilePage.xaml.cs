using Windows.UI.Xaml.Controls;
using Kliva.ViewModels;

namespace Kliva.Views
{
    public sealed partial class ProfilePage : Page
    {
        private ProfileViewModel ViewModel => DataContext as ProfileViewModel;

        public ProfilePage()
        {
            this.InitializeComponent();
        }
    }
}
