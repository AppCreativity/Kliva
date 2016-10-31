using Windows.UI.Xaml;
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

        public void ShowLoading(bool isLoading)
        {
            VisualStateManager.GoToState(this, isLoading ? "Loading" : "Normal", true);
        }

        private void OnNavigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var pageType = e.Content.GetType();
            ViewModelLocator.Get<SidePaneViewModel>().ShowHide(pageType);
        }
    }
}
