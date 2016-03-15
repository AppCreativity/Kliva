using Kliva.ViewModels.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kliva.Views
{
    public sealed partial class MainPage : Page
    {
        private IStravaViewModel ViewModel => DataContext as IStravaViewModel;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void UpdateVisualState(VisualState currentState)
        {
            ViewModel.CurrentState = currentState;
        }

        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            UpdateVisualState(e.NewState);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // We are only guaranteed to have the updated VisualState in Loaded
            UpdateVisualState(VisualStateGroup.CurrentState);
        }
    }
}
