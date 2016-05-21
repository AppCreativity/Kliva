using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Kliva.ViewModels;

namespace Kliva.Views
{
    public sealed partial class ClubsPage : Page
    {
        private ClubsViewModel ViewModel => DataContext as ClubsViewModel;

        public ClubsPage()
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
            UpdateVisualState(VisualStateGroup.CurrentState);
        }
    }
}
