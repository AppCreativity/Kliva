using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Messages;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Kliva.ViewModels.Interfaces;
using Kliva.Views;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Messaging;
using Kliva.Extensions;
using Kliva.Services;
using Microsoft.Practices.ServiceLocation;
using Kliva.Controls;
using System;

namespace Kliva.ViewModels
{
    public class MainViewModel : KlivaBaseViewModel, IStravaViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IStravaService _stravaService;
        private readonly IOService _ioService;

        private bool _viewModelLoaded = false;
        private Athlete _athlete;

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

        private string _filterText;
        public string FilterText
        {
            get { return _filterText; }
            set { Set(() => FilterText, ref _filterText, value); }
        }

        private ActivityIncrementalCollection _activityIncrementalCollection;
        public ActivityIncrementalCollection ActivityIncrementalCollection
        {
            get { return _activityIncrementalCollection; }
            set { Set(() => ActivityIncrementalCollection, ref _activityIncrementalCollection, value); }
        }
        private DeferringObservableCollection<ActivitySummary> _activityIncrementalCollection2;
        public DeferringObservableCollection<ActivitySummary> ActivityIncrementalCollection2
        {
            get { return _activityIncrementalCollection2; }
            set { Set(() => ActivityIncrementalCollection2, ref _activityIncrementalCollection2, value); }
        }

        private ActivitySummary _selectedActivity;
        public ActivitySummary SelectedActivity
        {
            get { return _selectedActivity; }
            set
            {
                if (Set(() => SelectedActivity, ref _selectedActivity, value) && value != null)
                    MessengerInstance.Send<ActivitySummaryMessage>(new ActivitySummaryMessage(_selectedActivity));
            }
        }

        private RelayCommand<string> _filterCommand;
        public RelayCommand<string> FilterCommand => _filterCommand ?? (_filterCommand = new RelayCommand<string>((item) =>
        {
            ActivityFeedFilter filter = Enum<ActivityFeedFilter>.Parse(item);
            _settingsService.SetActivityFeedFilterAsync(filter);

            ApplyActivityFeedFilter(filter);
        }));

        private RelayCommand _logoutCommand;
        public RelayCommand LogoutCommand => _logoutCommand ?? (_logoutCommand = new RelayCommand(async () => await Logout()));

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(ViewLoaded));

        private RelayCommand _recordCommand;
        public RelayCommand RecordCommand => _recordCommand ?? (_recordCommand = new RelayCommand(() => NavigationService.Navigate<RecordPage>()));

        //TODO: Glenn - We hooked this up twice, once in SidePaneViewModel and once in MainViewModel because of difference in UI on desktop ( sidebar ) and mobile ( bottom appbar )
        private RelayCommand _statisticsCommand;
        public RelayCommand StatisticsCommand => _statisticsCommand ?? (_statisticsCommand = new RelayCommand(() => NavigationService.Navigate<StatsPage>()));

        private RelayCommand _profileCommand;
        public RelayCommand ProfileCommand => _profileCommand ?? (_profileCommand = new RelayCommand(() => NavigationService.Navigate<ProfilePage>()));

        private RelayCommand _clubsCommand;
        public RelayCommand ClubsCommand => _clubsCommand ?? (_clubsCommand = new RelayCommand(() => NavigationService.Navigate<ClubsPage>()));

        //TODO: Glenn - We hooked this up twice, once in SidePaneViewModel and once in MainViewModel because of difference in UI on desktop ( sidebar ) and mobile ( bottom appbar )
        private RelayCommand _settingsCommand;
        public RelayCommand SettingsCommand => _settingsCommand ?? (_settingsCommand = new RelayCommand(() => NavigationService.Navigate<SettingsPage>()));

        private RelayCommand<ActivitySummary> _kudosCommand;
        public RelayCommand<ActivitySummary> KudosCommand => _kudosCommand ?? (_kudosCommand = new RelayCommand<ActivitySummary>(async (item) => await OnKudosAsync(item)));

        private RelayCommand<ActivitySummary> _commentCommand;
        public RelayCommand<ActivitySummary> CommentCommand => _commentCommand ?? (_commentCommand = new RelayCommand<ActivitySummary>(OnComment));

        public MainViewModel(INavigationService navigationService, ISettingsService settingsService, IStravaService stravaService, IOService ioService) : base(navigationService)
        {
            _settingsService = settingsService;
            _stravaService = stravaService;
            _ioService = ioService;
        }

        public void ActivityInvoked(ActivitySummary selectedActivity)
        {
            SelectedActivity = selectedActivity;
            TryNavigateToDetail();
        }

        private async Task Logout()
        {
            IsBusy = true;

            await _settingsService.RemoveStravaAccessTokenAsync();

            //Remove the current 'main page' back entry and navigate to the login page
            NavigationService.Navigate<LoginPage>();

            while(NavigationService.CanGoBack)
                NavigationService.RemoveBackEntry();

            IsBusy = false;
        }

        //TODO: Glenn - Can't remember, was this set up as async void for fire and forget reasons?
        private async void ViewLoaded()
        {
            if (!_viewModelLoaded)
            {
                var runTimeVersion = _settingsService.AppVersion;
                var storedVersion = await _settingsService.GetStoredAppVersionAsync();

                ActivityFeedFilter filter = await _settingsService.GetStoredActivityFeedFilterAsync();

                _athlete = await _stravaService.GetAthleteAsync();
                ApplyActivityFeedFilter(filter);

                //Show what's new information if the current version is newer than the stored version
                if (storedVersion.CompareTo(runTimeVersion) < 0)
                {
                    await _settingsService.SetAppVersionAsync(runTimeVersion);
                    //TODO: Glenn - Check loaded version with saved version in Settings, if different show what's new dialog and overwrite settings field
                    AppInfoDialog appInfo = new AppInfoDialog();
                    //TODO: Change the strings to enums or constants for the visual states
                    if (CurrentState.Name.Equals("Mobile", StringComparison.OrdinalIgnoreCase))
                        appInfo.FullSizeDesired = true;
                    else
                    {
                        appInfo.MinWidth = (double) (Window.Current.Bounds.Width * 90) / 100;
                        appInfo.MinHeight = (double)(Window.Current.Bounds.Height * 90) / 100;
                    }

                    await appInfo.ShowAsync();
                }

                _viewModelLoaded = true;
            }
        }

        private bool TryNavigateToDetail()
        {
            //TODO: Change the strings to enums or constants for the visual states
            if (CurrentState.Name == "Mobile" && SelectedActivity != null)
            {
                NavigationService.Navigate<ActivityDetailPage>();
                return true;
            }

            return false;
        }

        private void ApplyActivityFeedFilter(ActivityFeedFilter filter)
        {
            if (ActivityIncrementalCollection2 == null)
            {

                DeferringObservableCollection<ActivitySummary> activitySummaries;
                new ActivitySummaryService(_stravaService).Bind(out activitySummaries);
                ActivityIncrementalCollection2 = activitySummaries;
            }

            switch (filter)
            {
                case ActivityFeedFilter.All:
                    FilterText = "Showing all activities";
                    ActivityIncrementalCollection = new FriendActivityIncrementalCollection(_stravaService,
                        ActivityFeedFilter.All);
                    break;
                case ActivityFeedFilter.Followers:
                    FilterText = "Showing friends' activities";
                    ActivityIncrementalCollection = new FriendActivityIncrementalCollection(_stravaService,
                        ActivityFeedFilter.Friends);
                    break;
                case ActivityFeedFilter.My:
                    FilterText = "Showing my activities";
                    ActivityIncrementalCollection = new MyActivityIncrementalCollection(_stravaService);
                    break;
            }
        }

        private async Task OnKudosAsync(ActivitySummary activitySummary)
        {
            _athlete = _athlete ?? await _stravaService.GetAthleteAsync();
            var canGiveKudos = _athlete.Id != activitySummary.Athlete.Id && !activitySummary.HasKudoed;
            if (canGiveKudos)
            {
                IsBusy = true;
                await _stravaService.GiveKudosAsync(activitySummary.Id.ToString());
                activitySummary.HasKudoed = true;
                ++activitySummary.KudosCount;
                IsBusy = false;
            }

            //TODO: Glenn - when we are triggering a kudos, open kudos tab in detail page
            ActivityInvoked(activitySummary);
            ServiceLocator.Current.GetInstance<IMessenger>().Send<PivotMessage<ActivityPivots>>(new PivotMessage<ActivityPivots>(ActivityPivots.Kudos, true, true), Tokens.ActivityPivotMessage);
        }

        private void OnComment(ActivitySummary activitySummary)
        {
            // make sure we work with the swiped item
            // this could happen if we use the mouse to swipe and move the mouse outside the ListView and release it there
            if (activitySummary != SelectedActivity)
            {
                SelectedActivity = activitySummary;
            }

            //TODO: Glenn - the previous setting of SelectedActivity will trigger a load of that activity, if it was not yet loaded ( detail screen still empty )
            //TODO: Glenn - this loading is done async, hence we could have a potential problem that the user has entered a comment and tries to save it, before the activity has been loaded!
            MessengerInstance.Send<ActivitySummaryCommentMessage>(new ActivitySummaryCommentMessage(SelectedActivity));
        }
    }
}