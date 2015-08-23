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

        public RelayCommand LoginCommand { get; private set; }

        public LoginViewModel(INavigationService navigationService, IStravaService stravaService) : base(navigationService)
        {
            _stravaService = stravaService;
            _stravaService.StatusEvent += OnStravaStatusEvent;

            this.LoginCommand = new RelayCommand(async () => await _stravaService.GetAuthorizationCode());
        }

        private void OnStravaStatusEvent(object sender, Services.StravaServiceEventArgs e)
        {
            switch (e.Status)
            {
                case StravaServiceStatus.Success:
                    _stravaService.StatusEvent -= OnStravaStatusEvent;

                    //Remove the current 'login pageParam' back entry and navigate to the main page
                    _navigationService.Navigate<MainPage>();
                    _navigationService.RemoveBackEntry();

                    //this.IsBusy = false;
                    break;
                case StravaServiceStatus.Failed:
                    //this.IsBusy = false;

                    //await _messageBoxService.ShowAsync(Resources.Error_Login, Resources.Error);
                    break;
            }
        }
    }
}
