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

        private VisualState _currentState;
        public VisualState CurrentState
        {
            get { return _currentState; }
            set
            {
                if (!Equals(_currentState, value))
                {
                    _currentState = value;

                    if (_currentState.Name == "Mobile")
                    {
                        TryNavigateToDetail();
                    }
                }
            }
        }

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
                if (value != null)
                {
                    bool valueSet = Set(() => SelectedClub, ref _selectedClub, value);

                    switch (Enum<AppTarget>.Parse(CurrentState.Name))
                    {
                        case AppTarget.Mobile:
                            NavigationService.Navigate<ClubDetailPage>();
                            MessengerInstance.Send<ClubSummaryMessage>(new ClubSummaryMessage(_selectedClub));
                            break;
                        default:
                            if(valueSet)
                                MessengerInstance.Send<ClubSummaryMessage>(new ClubSummaryMessage(_selectedClub));
                            break;
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

        private bool TryNavigateToDetail()
        {
            if (CurrentState.Name == "Mobile" && SelectedClub != null)
            {
                NavigationService.Navigate<ClubDetailPage>();
                return true;
            }

            return false;
        }

        internal void ClubInvoked(ClubSummary clickedItem)
        {
            SelectedClub = clickedItem;
        }
    }
}
