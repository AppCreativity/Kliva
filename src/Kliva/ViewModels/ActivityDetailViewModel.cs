using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Kliva.Controls;
using Kliva.Extensions;
using Kliva.Messages;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Kliva.Views;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;
using Kliva.Services;

namespace Kliva.ViewModels
{
    public class ActivityDetailViewModel : KlivaBaseViewModel
    {
        private readonly IStravaService _stravaService;
        private readonly ILauncherService _launcherService;
        private Athlete _athlete;

        private Activity _selectedActivity;
        public Activity SelectedActivity
        {
            get { return _selectedActivity; }
            set
            {
                Set(() => SelectedActivity, ref _selectedActivity, value);
                RaisePropertyChanged(() => KudosCount);
                RaisePropertyChanged(() => CommentCount);
                RaisePropertyChanged(() => PhotoCount);
            }
        }

        private ObservableCollection<AthleteSummary> _kudos = new ObservableCollection<AthleteSummary>();
        public ObservableCollection<AthleteSummary> Kudos
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

        private ObservableCollection<AthleteSummary> _relatedAthletes = new ObservableCollection<AthleteSummary>();
        public ObservableCollection<AthleteSummary> RelatedAthletes
        {
            get { return _relatedAthletes; }
            set { Set(() => RelatedAthletes, ref _relatedAthletes, value); }
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

        private bool _hasKudoed;
        public bool HasKudoed
        {
            get { return _hasKudoed; }
            set { Set(() => HasKudoed, ref _hasKudoed, value); }
        }

        private bool _isEditEnabled = false;
        public bool IsEditEnabled
        {
            get { return _isEditEnabled; }
            set { Set(() => IsEditEnabled, ref _isEditEnabled, value); }
        }

        public int KudosCount => SelectedActivity?.KudosCount ?? 0;
        public int CommentCount => SelectedActivity?.CommentCount ?? 0;
        public int PhotoCount => SelectedActivity?.TotalPhotoCount ?? 0;

        private RelayCommand _kudosCommand;
        public RelayCommand KudosCommand => _kudosCommand ?? (_kudosCommand = new RelayCommand(async () => await OnKudosAsync(this.SelectedActivity.Id)));

        private RelayCommand _commentCommand;
        public RelayCommand CommentCommand => _commentCommand ?? (_commentCommand = new RelayCommand(async () => await OnCommentAsync(this.SelectedActivity.Id)));

        private RelayCommand _mapCommand;
        public RelayCommand MapCommand => _mapCommand ?? (_mapCommand = new RelayCommand(() => NavigationService.Navigate<MapPage>(SelectedActivity?.Map)));

        private RelayCommand _editCommand;
        public RelayCommand EditCommand => _editCommand ?? (_editCommand = new RelayCommand(async () => await OnEditAsync()));

        private RelayCommand _stravaCommand;
        public RelayCommand StravaCommand => _stravaCommand ?? (_stravaCommand = new RelayCommand(async () => await LaunchAsync()));

        private RelayCommand<ItemClickEventArgs> _athleteTappedCommand;
        public RelayCommand<ItemClickEventArgs> AthleteTappedCommand => _athleteTappedCommand ?? (_athleteTappedCommand = new RelayCommand<ItemClickEventArgs>(OnAthleteTapped));

        private RelayCommand<ItemClickEventArgs> _segmentTappedCommand;
        public RelayCommand<ItemClickEventArgs> SegmentTappedCommand => _segmentTappedCommand ?? (_segmentTappedCommand = new RelayCommand<ItemClickEventArgs>(OnSegmentTapped));        

        public ActivityDetailViewModel(INavigationService navigationService, IStravaService stravaService, ILauncherService launcherService) : base(navigationService)
        {
            _stravaService = stravaService;
            _launcherService = launcherService;
            MessengerInstance.Register<ActivitySummaryMessage>(this, async item => await LoadActivityDetails(item.ActivitySummary.Id.ToString()));

            MessengerInstance.Register<ActivitySummaryKudosMessage>(this, async item => await OnKudosAsync(item.ActivitySummary.Id));
            MessengerInstance.Register<ActivitySummaryCommentMessage>(this, async item => await OnCommentAsync(item.ActivitySummary.Id));
        }

        private async Task LoadActivityDetails(string activityId)
        {
            //We need to unload (remove the PropertyChanged event handler) from the 
            //UserMeasurementUnitStatisticsDetail items to avoid memory leaks
            //Not prety, but I don't see a better solution atm
            //ToDo: maybe use CleanUp in order to unwire eventhandlers?
            SelectedActivity?.Statistics?.Where(a => a is UserMeasurementUnitStatisticsDetail)
                .Select(a => a as UserMeasurementUnitStatisticsDetail).ToList().ForEach(a => a.Unload());

            Kudos.Clear();
            Comments.Clear();
            RelatedAthletes.Clear();

            //TODO: Glenn - Why aren't we receiving private activities?
            var activity = await _stravaService.GetActivityAsync(activityId, true);
            _athlete = await _stravaService.GetAthleteAsync();

            if (activity != null)
            {
                SelectedActivity = activity;

                if (activity.KudosCount > 0 && activity.Kudos != null && activity.Kudos.Any())
                {                    
                    foreach (AthleteSummary kudo in activity.Kudos)
                        Kudos.Add(kudo);
                }
                
                if (activity.CommentCount > 0 && activity.Comments != null && activity.Comments.Any())
                {
                    foreach(Comment comment in activity.Comments)
                        Comments.Add(comment);
                }
                
                if (activity.OtherAthleteCount > 0 && activity.RelatedActivities != null && activity.RelatedActivities.Any())
                {
                    foreach (ActivitySummary relatedActivity in activity.RelatedActivities)
                        RelatedAthletes.Add(relatedActivity.Athlete);
                }

                //Currently the Public API of Strava will not give us Segments info for 'other' athletes then the one logged in
                HasSegments = SelectedActivity.SegmentEfforts != null;

                //Currently the Public API of Strava will not give us the Photo links for 'other' athletes then the one logged in
                //But we do get the photo count, so we also need to verify the current user vs the one from the activity
                HasPhotos = _athlete.Id == SelectedActivity.Athlete.Id && SelectedActivity.TotalPhotoCount > 0;
                HasKudoed = _athlete.Id == SelectedActivity.Athlete.Id || SelectedActivity.HasKudoed;
                IsEditEnabled = _athlete.Id == SelectedActivity.Athlete.Id;

                //TODO: Glenn - Why oh why are we not yet able to show/hide PivotItems through Visibility bindable
                ServiceLocator.Current.GetInstance<IMessenger>().Send<PivotMessage<ActivityPivots>>(new PivotMessage<ActivityPivots>(ActivityPivots.Segments, HasSegments), Tokens.ActivityPivotMessage);
                ServiceLocator.Current.GetInstance<IMessenger>().Send<PivotMessage<ActivityPivots>>(new PivotMessage<ActivityPivots>(ActivityPivots.Photos, HasPhotos), Tokens.ActivityPivotMessage);

                ServiceLocator.Current.GetInstance<IMessenger>()
                    .Send<PolylineMessage>(!string.IsNullOrEmpty(activity?.Map.SummaryPolyline)
                        ? new PolylineMessage(activity.Map.GeoPositions)
                        : new PolylineMessage(new List<BasicGeoposition>()), Tokens.ActivityPolylineMessage);
            }
        }

        private async Task OnKudosAsync(long activityId)
        {
            await _stravaService.GiveKudosAsync(activityId.ToString());
            await LoadActivityDetails(activityId.ToString());
            ServiceLocator.Current.GetInstance<IMessenger>().Send<PivotMessage<ActivityPivots>>(new PivotMessage<ActivityPivots>(ActivityPivots.Kudos, true, true), Tokens.ActivityPivotMessage);
        }

        private async Task OnCommentAsync(long activityId)
        {
            CommentContentDialog dialog = new CommentContentDialog();
            dialog.AdjustSize();

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && !string.IsNullOrEmpty(dialog.Description))
            {
                await _stravaService.PostComment(activityId.ToString(), dialog.Description);
                await LoadActivityDetails(activityId.ToString());
                ServiceLocator.Current.GetInstance<IMessenger>().Send<PivotMessage<ActivityPivots>>(new PivotMessage<ActivityPivots>(ActivityPivots.Comments, true, true), Tokens.ActivityPivotMessage);
            }
        }

        private async Task OnEditAsync()
        {
            List<GearSummary> gear = null;

            switch (SelectedActivity.Type)
            {
                case ActivityType.Ride:
                case ActivityType.EBikeRide:
                    gear = _athlete.Bikes.Cast<GearSummary>().ToList();
                    break;
                case ActivityType.Run:
                    gear = _athlete.Shoes.Cast<GearSummary>().ToList();
                    break;
            }

            EditContentDialog dialog = new EditContentDialog(SelectedActivity.Name, SelectedActivity.IsCommute, SelectedActivity.IsPrivate, gear);
            dialog.AdjustSize();

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && !string.IsNullOrEmpty(dialog.ActivityName))
            {
                await _stravaService.PutUpdate(SelectedActivity.Id.ToString(), dialog.ActivityName, dialog.ActivityCommute, dialog.ActivityPrivate, dialog.SelectedGear.GearID);
                await LoadActivityDetails(SelectedActivity.Id.ToString());
            }
        }

        private Task LaunchAsync()
        {
            string activityUri = $"{Endpoints.PublicActivity}/{SelectedActivity.Id}";
            return _launcherService.LaunchUriAsync(new Uri(activityUri));
        }
    }
}