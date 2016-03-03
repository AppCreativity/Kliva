using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Helpers;
using Kliva.Messages;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Kliva.Views;

namespace Kliva.ViewModels
{
    public class ClubsViewModel : KlivaBaseViewModel
    {
        private readonly IStravaService _stravaService;

        private bool _viewModelLoaded = false;

        public VisualState CurrentState { get; set; }

        private ObservableCollection<ClubSummary> _clubs = new ObservableCollection<ClubSummary>();
        public ObservableCollection<ClubSummary> Clubs
        {
            get { return _clubs; }
            set { Set(() => Clubs, ref _clubs, value); }
        }

        private ClubSummary _selectedClub;
        public ClubSummary SelectedClub
        {
            get { return _selectedClub; }
            set
            {
                if (Set(() => SelectedClub, ref _selectedClub, value) && value != null)
                {
                    {
                        switch (Enum<AppTarget>.Parse(CurrentState.Name))
                        {
                            case AppTarget.Mobile:
                                _navigationService.Navigate<ClubDetailPage>();
                                break;
                        }

                        MessengerInstance.Send<ClubSummaryMessage>(new ClubSummaryMessage(_selectedClub));
                    }
                }
            }
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
