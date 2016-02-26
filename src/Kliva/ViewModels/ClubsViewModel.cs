using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class ClubsViewModel : KlivaBaseViewModel
    {
        private readonly IStravaService _stravaService;

        private bool _viewModelLoaded = false;

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(ViewLoaded));

        public ClubsViewModel(INavigationService navigationService, IStravaService stravaService) : base(navigationService)
        {
            _stravaService = stravaService;
        }

        private void ViewLoaded()
        {
            if (!_viewModelLoaded)
            {
                _viewModelLoaded = true;
            }
        }
    }
}
