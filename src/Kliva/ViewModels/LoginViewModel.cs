using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Services;
using Kliva.Services.Interfaces;
using Kliva.Views;

namespace Kliva.ViewModels
{
    public class LoginViewModel : KlivaBaseViewModel
    {
        private IStravaService _stravaService;
        private IMessageBoxService _messageBoxService;

        public RelayCommand LoginCommand { get; private set; }

        public LoginViewModel(INavigationService navigationService, IMessageBoxService messageBoxService, IStravaService stravaService) : base(navigationService)
        {
            _stravaService = stravaService;
            _messageBoxService = messageBoxService;
            _stravaService.StatusEvent += OnStravaStatusEvent;

            this.LoginCommand = new RelayCommand(async () => await _stravaService.GetAuthorizationCode());
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

                    this.IsBusy = false;
                    break;
                case StravaServiceStatus.Failed:
                    this.IsBusy = false;

                    //TODO: Glenn - Add more text value through resources
                    await _messageBoxService.ShowAsync("Something happend...", "Error");
                    break;
            }
        }
    }
}
