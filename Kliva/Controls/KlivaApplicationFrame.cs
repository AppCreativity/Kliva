using Kliva.ViewModels;
using Windows.UI.Xaml.Controls;

namespace Kliva.Controls
{
    public class KlivaApplicationFrame : Frame
    {
        public KlivaApplicationFrame()
        {
            Navigated += OnNavigated;
        }

        private void OnNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var pageType = e.Content.GetType();
            ViewModelLocator.Get<SidePaneViewModel>().ShowHide(pageType);
        }
    }
}
