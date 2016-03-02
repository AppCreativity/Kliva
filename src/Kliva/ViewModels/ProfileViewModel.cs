using System.Collections.ObjectModel;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Models;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class ProfileViewModel : KlivaBaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IStravaService _stravaService;

        private bool _viewModelLoaded = false;

        public ProfileViewModel(INavigationService navigationService, IStravaService stravaService)
            : base(navigationService)
        {
            _stravaService = stravaService;
            _navigationService = navigationService;
        }

        private Athlete _athlete;
        public Athlete Athlete
        {
            get { return _athlete; }
            set { Set(() => Athlete, ref _athlete, value); }
        }

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(ViewLoaded));

        private async void ViewLoaded()
        {
            // clear old value // TODO configure ioc to give a new VM per call
            Athlete = null;
            
            var currentParameter = _navigationService.CurrentParameter;
            if (currentParameter == null)
            {
                Athlete = await _stravaService.GetAthleteAsync();
            }
            else
            {

            }
        }
    }
}
