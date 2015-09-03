using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Kliva.ViewModels.Interfaces;
using Kliva.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Kliva.ViewModels
{
    public class MainViewModel : KlivaBaseViewModel, IStravaViewModel
    {
        private ISettingsService _settingsService;
        private IStravaService _stravaService;

        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set { Set(() => IsPaneOpen, ref _isPaneOpen, value); }
        }

        private ObservableCollection<ActivitySummary> _activities = new ObservableCollection<ActivitySummary>();
        public ObservableCollection<ActivitySummary> Activities
        {
            get { return _activities; }
            set { Set(() => Activities, ref _activities, value); }
        }

        private RelayCommand _logoutCommand;
        public RelayCommand LogoutCommand => _logoutCommand ?? (_logoutCommand = new RelayCommand(async () => await this.Logout()));

        private RelayCommand _hamburgerCommand;
        public RelayCommand HamburgerCommand => _hamburgerCommand ?? (_hamburgerCommand = new RelayCommand(() => this.IsPaneOpen = !this.IsPaneOpen));

        private RelayCommand _settingsCommand;
        public RelayCommand SettingsCommand => _settingsCommand ?? (_settingsCommand = new RelayCommand(() => _navigationService.Navigate<SettingsPage>()));

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(async () => await this.ViewLoaded()));

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

        private async Task ViewLoaded()
        {
            this.Activities.Clear();
            var activities = await _stravaService.GetActivitiesWithAthletesAsync(0, 30);
            foreach (ActivitySummary activity in activities)
                this.Activities.Add(activity);
        }
    }
}