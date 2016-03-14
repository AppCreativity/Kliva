using System.Collections.Generic;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Messages;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Kliva.ViewModels.Interfaces;
using Kliva.Views;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Messaging;
using Kliva.Helpers;
using Microsoft.Practices.ServiceLocation;

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

                    if (_currentState.Name == "Mobile")
                    {
                        TryNavigateToDetail();
                    }
                }
            }
        }

        private string _filterText;
        public string FilterText
        {
            get { return _filterText; }
            set { Set(() => FilterText, ref _filterText, value); }
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

        private RelayCommand<string> _filterCommand;
        public RelayCommand<string> FilterCommand => _filterCommand ?? (_filterCommand = new RelayCommand<string>((item) =>
        {
            ActivityFeedFilter filter = Enum<ActivityFeedFilter>.Parse(item);
            _settingsService.SetActivityFeedFilterAsync(filter);

            switch (filter)
            {
                case ActivityFeedFilter.All:
                    FilterText = "Showing all activities";
                    ActivityIncrementalCollection = new FriendActivityIncrementalCollection(_stravaService, ActivityFeedFilter.All);
                    break;
                case ActivityFeedFilter.Followers:
                    FilterText = "Showing friends' activities";
                    ActivityIncrementalCollection = new FriendActivityIncrementalCollection(_stravaService, ActivityFeedFilter.Friends);
                    break;
                case ActivityFeedFilter.My:
                    FilterText = "Showing my activities";
                    ActivityIncrementalCollection = new MyActivityIncrementalCollection(_stravaService);
                    break;
            }
        }));

        private RelayCommand _logoutCommand;
        public RelayCommand LogoutCommand => _logoutCommand ?? (_logoutCommand = new RelayCommand(async () => await this.Logout()));

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(ViewLoaded));

        //TODO: Glenn - We hooked this up twice, once in SidePaneViewModel and once in MainViewModel because of difference in UI on desktop ( sidebar ) and mobile ( bottom appbar )
        private RelayCommand _statisticsCommand;
        public RelayCommand StatisticsCommand => _statisticsCommand ?? (_statisticsCommand = new RelayCommand(() => NavigationService.Navigate<StatsPage>()));

        private RelayCommand _profileCommand;
        public RelayCommand ProfileCommand => _profileCommand ?? (_profileCommand = new RelayCommand(() => NavigationService.Navigate<ProfilePage>()));

        private RelayCommand _clubsCommand;
        public RelayCommand ClubsCommand => _clubsCommand ?? (_clubsCommand = new RelayCommand(() => NavigationService.Navigate<ClubsPage>()));

        //TODO: Glenn - We hooked this up twice, once in SidePaneViewModel and once in MainViewModel because of difference in UI on desktop ( sidebar ) and mobile ( bottom appbar )
        private RelayCommand _settingsCommand;
        public RelayCommand SettingsCommand => _settingsCommand ?? (_settingsCommand = new RelayCommand(() => NavigationService.Navigate<SettingsPage>()));

        public MainViewModel(INavigationService navigationService, ISettingsService settingsService, IStravaService stravaService) : base(navigationService)
        {
            _settingsService = settingsService;
            _stravaService = stravaService;

            //TODO: Glenn - store selected filter in settings!            
            FilterText = "Showing all activities";
        }

        private async Task Logout()
        {
            //this.IsBusy = true;

            await _settingsService.RemoveStravaAccessTokenAsync();

            //Remove the current 'main page' back entry and navigate to the login page
            NavigationService.Navigate<LoginPage>();
            NavigationService.RemoveBackEntry();

            //this.IsBusy = false;
        }

        private async void ViewLoaded()
        {
            if (!_viewModelLoaded)
            {
                ActivityFeedFilter filter = await _settingsService.GetStoredActivityFeedFilterAsync();

                var athlete = await _stravaService.GetAthleteAsync();
                ActivityIncrementalCollection = new FriendActivityIncrementalCollection(_stravaService, filter);
                _viewModelLoaded = true;
            }
        }

        private bool TryNavigateToDetail()
        {
            //TODO: Change the strings to enums or constants for the visual states
            if (CurrentState.Name == "Mobile" && SelectedActivity != null)
            {
                NavigationService.Navigate<ActivityDetailPage>();
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