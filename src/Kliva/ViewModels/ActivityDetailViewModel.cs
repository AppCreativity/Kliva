using System.Threading.Tasks;
using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using Kliva.Messages;
using Kliva.Models;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class ActivityDetailViewModel : KlivaBaseViewModel
    {
        private readonly IStravaService _stravaService;

        private Activity _selectedActivity;
        public Activity SelectedActivity
        {
            get { return _selectedActivity; }
            set { Set(() => SelectedActivity, ref _selectedActivity, value); }
        }

        public VisualState CurrentState { get; set; }

        public ActivityDetailViewModel(INavigationService navigationService, IStravaService stravaService) : base(navigationService)
        {
            _stravaService = stravaService;
            MessengerInstance.Register<ActivitySummaryMessage>(this, async item => await LoadActivityDetails(item));
        }

        private async Task LoadActivityDetails(ActivitySummaryMessage message)
        {
            var activity = await _stravaService.GetActivityAsync(message.ActivitySummary.Id.ToString(), true);
            if (activity != null)
                SelectedActivity = activity;
        }
    }
}