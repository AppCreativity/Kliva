using System;
using System.Linq;
using Kliva.ViewModels.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Cimbalino.Toolkit.Extensions;
using System.Collections.Generic;

namespace Kliva.Views
{
    public sealed partial class MainPage : Page
    {
        private ListView _activityList;

        private IStravaViewModel ViewModel => DataContext as IStravaViewModel;

        public MainPage()
        {
            this.InitializeComponent();

            _activityList = this.GetVisualDescendents<ListView>().FirstOrDefault(item => item.Name.Equals("ActivityList", StringComparison.OrdinalIgnoreCase));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _activityList.SelectedIndex = -1;
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
            // We are only guarunteed to have the updated VisualState in Loaded
            UpdateVisualState(VisualStateGroup.CurrentState);
        }
    }
}
