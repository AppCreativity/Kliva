using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Kliva.ViewModels;

namespace Kliva.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        private ProfileViewModel ViewModel => DataContext as ProfileViewModel;

        public ProfilePage()
        {
            this.InitializeComponent();
        }
    }
}
