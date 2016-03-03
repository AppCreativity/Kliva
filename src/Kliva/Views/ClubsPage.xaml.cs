using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Cimbalino.Toolkit.Extensions;
using Kliva.ViewModels;

namespace Kliva.Views
{
    public sealed partial class ClubsPage : Page
    {
        private readonly ListView _clubList;

        private ClubsViewModel ViewModel => DataContext as ClubsViewModel;

        public ClubsPage()
        {
            this.InitializeComponent();
            _clubList = this.GetVisualDescendents<ListView>().FirstOrDefault(item => item.Name.Equals("ClubList", StringComparison.OrdinalIgnoreCase));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdateVisualState(VisualStateGroup.CurrentState);
            _clubList.SelectedIndex = -1;
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
