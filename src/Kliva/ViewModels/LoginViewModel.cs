using System;
using System.Threading.Tasks;
using Windows.System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Controls;
using Kliva.Models;
using Kliva.Services;
using Kliva.Services.Interfaces;
using Kliva.Views;

namespace Kliva.ViewModels
{
    public class LoginViewModel : KlivaBaseViewModel
    {
        private IStravaService _stravaService;
        private IMessageBoxService _messageBoxService;

        private RelayCommand _loginCommand;
        public RelayCommand LoginCommand => _loginCommand ?? (_loginCommand = new RelayCommand(async () =>
        {
            IsBusy = true;
            await _stravaService.GetAuthorizationCode();
            IsBusy = false;
        }));

        private RelayCommand _newAccountCommand;
        public RelayCommand NewAccountCommand => _newAccountCommand ?? (_newAccountCommand = new RelayCommand(async () => await Launcher.LaunchUriAsync(new Uri(Constants.STRAVA_NEW_ACCOUNT))));

        public LoginViewModel(INavigationService navigationService, IMessageBoxService messageBoxService, IStravaService stravaService) : base(navigationService)
        {
            _stravaService = stravaService;
            _messageBoxService = messageBoxService;
            _stravaService.StatusEvent += OnStravaStatusEvent;
        }

        private async void OnStravaStatusEvent(object sender, Services.StravaServiceEventArgs e)
        {
            switch (e.Status)
            {
                case StravaServiceStatus.Success:
                    _stravaService.StatusEvent -= OnStravaStatusEvent;                    

                    //Remove the current 'login page' back entry and navigate to the main page
                    NavigationService.Navigate<MainPage>();
                    NavigationService.RemoveBackEntry();                    

                    IsBusy = false;
                    break;
                case StravaServiceStatus.Failed:
                    IsBusy = false;

                    //TODO: Glenn - Add more text value through resources
                    await _messageBoxService.ShowAsync("Something happend...", "Error");
                    break;
            }
        }
    }
}
