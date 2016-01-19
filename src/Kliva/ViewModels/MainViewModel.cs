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

namespace Kliva.ViewModels
{
    public class MainViewModel : KlivaBaseViewModel, IStravaViewModel
    {
        private ISettingsService _settingsService;
        private IStravaService _stravaService;        

        public VisualState CurrentState { get; set; }

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
                    //TODO: Change the strings to enums or constants for the visual states
                    switch (CurrentState.Name)
                    {
                        case "Mobile":
                            _navigationService.Navigate<ActivityDetailPage>();
                            MessengerInstance.Send<ActivitySummaryMessage>(new ActivitySummaryMessage(_selectedActivity));
                            break;
                    }

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

        //TODO: Glenn - We hooked this up twice, once in SidePaneViewModel and once in MainViewModel
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
            //this.Activities.Clear();
            //var activities = await _stravaService.GetActivitiesWithAthletesAsync(0, 30);

            //foreach (ActivitySummary activity in activities)
            //    this.Activities.Add(activity);

            //TODO: Glenn - Do we always need to new this here?
            ActivityIncrementalCollection = new ActivityIncrementalCollection(_stravaService);
        }
    }
}