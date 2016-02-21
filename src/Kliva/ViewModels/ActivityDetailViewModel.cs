using System.Threading.Tasks;
using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Messaging;
using Kliva.Messages;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Microsoft.Practices.ServiceLocation;

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

        private bool _hasSegments;
        public bool HasSegments
        {
            get { return _hasSegments; }
            set { Set(() => HasSegments, ref _hasSegments, value); }
        }

        private bool _hasPhotos;
        public bool HasPhotos
        {
            get { return _hasPhotos; }
            set { Set(() => HasPhotos, ref _hasPhotos, value); }
        }

        public ActivityDetailViewModel(INavigationService navigationService, IStravaService stravaService) : base(navigationService)
        {
            _stravaService = stravaService;
            MessengerInstance.Register<ActivitySummaryMessage>(this, async item => await LoadActivityDetails(item));
        }

        private async Task LoadActivityDetails(ActivitySummaryMessage message)
        {
            var activity = await _stravaService.GetActivityAsync(message.ActivitySummary.Id.ToString(), true);
            var athlete = await _stravaService.GetAthleteAsync();

            if (activity != null)
            {
                SelectedActivity = activity;

                //Currently the Public API of Strava will not give us Segments info for 'other' athletes then the one logged in
                HasSegments = SelectedActivity.SegmentEfforts != null;

                //Currently the Public API of Strava will not give us the Photo links for 'other' athletes then the one logged in
                //But we do get the photo count, so we also need to verify the current user vs the one from the activity
                HasPhotos = athlete.Id == SelectedActivity.Athlete.Id && SelectedActivity.TotalPhotoCount > 0;

                //TODO: Glenn - Why oh why are we not yet able to show/hide PivotItems through Visibility bindable
                ServiceLocator.Current.GetInstance<IMessenger>().Send<PivotMessage>(new PivotMessage(Pivots.Segments, this.HasSegments));
                ServiceLocator.Current.GetInstance<IMessenger>().Send<PivotMessage>(new PivotMessage(Pivots.Photos, this.HasPhotos));
            }
        }
    }
}