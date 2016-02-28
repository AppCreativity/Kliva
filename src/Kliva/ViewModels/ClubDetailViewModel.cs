using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using Kliva.Messages;
using Kliva.Models;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class ClubDetailViewModel : KlivaBaseViewModel
    {
        private readonly IStravaService _stravaService;

        private Club _selectedClub;
        public Club SelectedClub
        {
            get { return _selectedClub; }
            set { Set(() => SelectedClub, ref _selectedClub, value); }
        }

        public ClubDetailViewModel(INavigationService navigationService, IStravaService stravaService) : base(navigationService)
        {
            _stravaService = stravaService;
            MessengerInstance.Register<ClubSummaryMessage>(this, async item => await LoadClubDetails(item.Club.Id.ToString()));
        }

        private async Task LoadClubDetails(string clubId)
        {
            var club = await _stravaService.GetClubAsync(clubId);

            if (club != null)
            {
                SelectedClub = club;
            }
        }
    }
}
