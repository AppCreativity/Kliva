using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
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

        private ObservableCollection<Athlete> _kudos = new ObservableCollection<Athlete>();
        public ObservableCollection<Athlete> Kudos
        {
            get { return _kudos; }
            set { Set(() => Kudos, ref _kudos, value); }
        } 

        private ObservableCollection<Comment> _comments = new ObservableCollection<Comment>();
        public ObservableCollection<Comment> Comments
        {
            get { return _comments; }
            set { Set(() => Comments, ref _comments, value); }
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

        private RelayCommand _kudosCommand;
        public RelayCommand KudosCommand => _kudosCommand ?? (_kudosCommand = new RelayCommand(async () => await OnKudos()));

        public ActivityDetailViewModel(INavigationService navigationService, IStravaService stravaService) : base(navigationService)
        {
            _stravaService = stravaService;
            MessengerInstance.Register<ActivitySummaryMessage>(this, async item => await LoadActivityDetails(item.ActivitySummary.Id.ToString()));
        }

        private async Task LoadActivityDetails(string activityId)
        {
            var activity = await _stravaService.GetActivityAsync(activityId, true);
            var athlete = await _stravaService.GetAthleteAsync();

            if (activity != null)
            {
                SelectedActivity = activity;

                Kudos.Clear();
                if (activity.KudosCount > 0 && activity.Kudos != null && activity.Kudos.Any())
                {                    
                    foreach (Athlete kudo in activity.Kudos)
                        Kudos.Add(kudo);
                }

                Comments.Clear();
                if (activity.CommentCount > 0 && activity.Comments != null && activity.Comments.Any())
                {
                    foreach(Comment comment in activity.Comments)
                        Comments.Add(comment);
                }

                //Currently the Public API of Strava will not give us Segments info for 'other' athletes then the one logged in
                HasSegments = SelectedActivity.SegmentEfforts != null;

                //Currently the Public API of Strava will not give us the Photo links for 'other' athletes then the one logged in
                //But we do get the photo count, so we also need to verify the current user vs the one from the activity
                HasPhotos = athlete.Id == SelectedActivity.Athlete.Id && SelectedActivity.TotalPhotoCount > 0;

                //TODO: Glenn - Why oh why are we not yet able to show/hide PivotItems through Visibility bindable
                ServiceLocator.Current.GetInstance<IMessenger>().Send<PivotMessage>(new PivotMessage(Pivots.Segments, HasSegments));
                ServiceLocator.Current.GetInstance<IMessenger>().Send<PivotMessage>(new PivotMessage(Pivots.Photos, HasPhotos));
            }
        }

        private async Task OnKudos()
        {
            await _stravaService.GiveKudos(SelectedActivity.Id.ToString());
            await LoadActivityDetails(SelectedActivity.Id.ToString());
            ServiceLocator.Current.GetInstance<IMessenger>().Send<PivotMessage>(new PivotMessage(Pivots.Kudos, true, true));
        }
    }
}