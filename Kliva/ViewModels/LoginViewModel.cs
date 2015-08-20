using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class LoginViewModel : KlivaBaseViewModel
    {
        private IStravaService _stravaService;

        public RelayCommand LoginCommand { get; private set; }

        public LoginViewModel(INavigationService navigationService, IStravaService stravaService) : base(navigationService)
        {
            _stravaService = stravaService;

            this.LoginCommand = new RelayCommand(async () => await _stravaService.GetAuthorizationCode());
        }
    }
}
