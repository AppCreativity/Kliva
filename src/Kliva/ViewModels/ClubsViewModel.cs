using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Models;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class ClubsViewModel : KlivaBaseViewModel
    {
        private readonly IStravaService _stravaService;

        private bool _viewModelLoaded = false;

        private ObservableCollection<ClubSummary> _clubs = new ObservableCollection<ClubSummary>();
        public ObservableCollection<ClubSummary> Clubs
        {
            get { return _clubs; }
            set { Set(() => Clubs, ref _clubs, value); }
        }

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(async () => await ViewLoaded()));

        public ClubsViewModel(INavigationService navigationService, IStravaService stravaService) : base(navigationService)
        {
            _stravaService = stravaService;
        }

        private async Task ViewLoaded()
        {
            if (!_viewModelLoaded)
            {
                Clubs.Clear();
                List<ClubSummary> clubList = await _stravaService.GetClubsAsync();
                if(clubList != null && clubList.Any())
                    foreach (ClubSummary clubSummary in clubList)
                        Clubs.Add(clubSummary);

                _viewModelLoaded = true;
            }
        }
    }
}
