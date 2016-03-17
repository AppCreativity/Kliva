using Windows.UI.Xaml.Controls;
using Kliva.Controls;
using Kliva.ViewModels;

namespace Kliva.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ActivityDetailPage : DetailPageBase
    {
        private ActivityDetailViewModel ViewModel => DataContext as ActivityDetailViewModel;

        public ActivityDetailPage()
        {
            this.InitializeComponent();
        }
    }
}
