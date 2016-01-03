using Kliva.ViewModels.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Cimbalino.Toolkit.Extensions;

namespace Kliva.Views
{
    public sealed partial class MainPage : Page
    {
        private IStravaViewModel ViewModel => DataContext as IStravaViewModel;

        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdateVisualState(VisualStateGroup.CurrentState);

            //TODO: Glenn - Could be we need to 'unselect' current selected activity when we return to the main page. Search it with VisualTree
            //var t = this.GetVisualDescendents<ListView>();
        }

        private void UpdateVisualState(VisualState currentState)
        {
            ViewModel.CurrentState = currentState;
        }

        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateVisualState(e.NewState);
        }
    }
}
