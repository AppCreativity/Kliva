using Windows.UI.Xaml.Controls;
using Kliva.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;

namespace Kliva.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ActivityDetailPage : Page
    {
        private ActivityDetailViewModel ViewModel => DataContext as ActivityDetailViewModel;

        public ActivityDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SizeChanged += Current_SizeChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            TryNavigateBackForDesktopState();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // If we navigated back to this page in desktop mode, we should not show it and go back to the
            // main page instead.
            if (!TryNavigateBackForDesktopState())
            {
                // Realize the elements that we have deferred for this case
                FindName(nameof(PageRoot));
            }
        }

        private bool TryNavigateBackForDesktopState()
        {
            if (Window.Current.Bounds.Width >= 720)
            {
                Frame.GoBack();
                return true;
            }

            return false;
        }
    }
}
