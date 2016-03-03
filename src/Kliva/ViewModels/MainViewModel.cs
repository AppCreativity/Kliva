using System.Collections.Generic;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Messages;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Kliva.ViewModels.Interfaces;
using Kliva.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System.Diagnostics;
using System;

namespace Kliva.ViewModels
{
    public class MainViewModel : KlivaBaseViewModel, IStravaViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IStravaService _stravaService;

        private bool _viewModelLoaded = false;

        private VisualState _currentState;
        public VisualState CurrentState
        {
            get { return _currentState; }
            set
            {
                if (!Equals(_currentState, value))
                {
                    _currentState = value;
                    // If we just switched to the mobile state we should collapse to the detail view
                    TryNavigateToDetail();
                }
            }
        }

        private ObservableCollection<ActivitySummary> _activities = new ObservableCollection<ActivitySummary>();
        public ObservableCollection<ActivitySummary> Activities
        {
            get { return _activities; }
            set { Set(() => Activities, ref _activities, value); }
        }

        private ActivityIncrementalCollection _activityIncrementalCollection;
        public ActivityIncrementalCollection ActivityIncrementalCollection
        {
            get { return _activityIncrementalCollection; }
            set { Set(() => ActivityIncrementalCollection, ref _activityIncrementalCollection, value); }
        }

        private ActivitySummary _selectedActivity;
        public ActivitySummary SelectedActivity
        {
            get { return _selectedActivity; }
            set
            {
                if (Set(() => SelectedActivity, ref _selectedActivity, value) && value != null)
                {
                    MessengerInstance.Send<ActivitySummaryMessage>(new ActivitySummaryMessage(_selectedActivity));

                    if (!string.IsNullOrEmpty(SelectedActivity?.Map.SummaryPolyline))
                        ServiceLocator.Current.GetInstance<IMessenger>().Send<ActivityPolylineMessage>(new ActivityPolylineMessage(SelectedActivity.Map.GeoPositions));
                    else
                        ServiceLocator.Current.GetInstance<IMessenger>().Send<ActivityPolylineMessage>(new ActivityPolylineMessage(new List<BasicGeoposition>()));
                }
            }
        }

        private RelayCommand _logoutCommand;
        public RelayCommand LogoutCommand => _logoutCommand ?? (_logoutCommand = new RelayCommand(async () => await this.Logout()));

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(ViewLoaded));

        //TODO: Glenn - We hooked this up twice, once in SidePaneViewModel and once in MainViewModel because of difference in UI on desktop ( sidebar ) and mobile ( bottom appbar )
        private RelayCommand _clubsCommand;
        public RelayCommand ClubsCommand => _clubsCommand ?? (_clubsCommand = new RelayCommand(() => _navigationService.Navigate<ClubsPage>()));

        //TODO: Glenn - We hooked this up twice, once in SidePaneViewModel and once in MainViewModel because of difference in UI on desktop ( sidebar ) and mobile ( bottom appbar )
        private RelayCommand _settingsCommand;
        public RelayCommand SettingsCommand => _settingsCommand ?? (_settingsCommand = new RelayCommand(() => _navigationService.Navigate<SettingsPage>()));

        public MainViewModel(INavigationService navigationService, ISettingsService settingsService, IStravaService stravaService) : base(navigationService)
        {
            _settingsService = settingsService;
            _stravaService = stravaService;            
        }

        private async Task Logout()
        {
            //this.IsBusy = true;

            await _settingsService.RemoveStravaAccessToken();

            //Remove the current 'main page' back entry and navigate to the login page
            _navigationService.Navigate<LoginPage>();
            _navigationService.RemoveBackEntry();

            //this.IsBusy = false;
        }

        private void ViewLoaded()
        {
            if (!_viewModelLoaded)
            {
                ActivityIncrementalCollection = new ActivityIncrementalCollection(_stravaService);
                _viewModelLoaded = true;
            }
        }

        private bool TryNavigateToDetail()
        {
            //TODO: Change the strings to enums or constants for the visual states
            if (CurrentState.Name == "Mobile" && SelectedActivity != null)
            {
                _navigationService.Navigate<ActivityDetailPage>();
                return true;
            }

            return false;
        }

        public void ActivityInvoked(ActivitySummary selectedActivity)
        {
            SelectedActivity = selectedActivity;
            TryNavigateToDetail();
        }
    }
}