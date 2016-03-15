using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Kliva.Controls
{
    public class DetailPageBase : Page
    {
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

        protected void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // If we navigated back to this page in desktop mode, we should not show it and go back to the
            // main page instead.
            if (!TryNavigateBackForDesktopState())
            {
                // Realize the elements that we have deferred for this case
                FindName("PageRoot");
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
