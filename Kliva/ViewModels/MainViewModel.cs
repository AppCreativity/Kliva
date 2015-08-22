using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Views;
using System.Threading.Tasks;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class MainViewModel : KlivaBaseViewModel
    {
        private ISettingsService _settingsService;

        private RelayCommand _logoutCommand;
        public RelayCommand LogoutCommand
        {
            get
            {
                return _logoutCommand ?? (_logoutCommand = new RelayCommand(async () => await this.Logout()));
            }
        }

        public MainViewModel(INavigationService navigationService, ISettingsService settingsService) : base(navigationService)
        {
            _settingsService = settingsService;
        }

        private async Task Logout()
        {
            //this.IsBusy = true;

            await _settingsService.RemoveStravaAccessToken();

            //Remove the current 'login pageParam' back entry and navigate to the login page
            _navigationService.Navigate<LoginPage>();
            _navigationService.RemoveBackEntry();

            //this.IsBusy = false;
        }

    }
}