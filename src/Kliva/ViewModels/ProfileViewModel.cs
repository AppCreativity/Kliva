using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Kliva.Models;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class ProfileViewModel : KlivaBaseViewModel
    {
        private readonly IStravaService _stravaService;
        private readonly ISettingsService _settingsService;

        private string _currentAthleteId;

        public ProfileViewModel(INavigationService navigationService, IStravaService stravaService, ISettingsService settingsService)
            : base(navigationService)
        {
            _stravaService = stravaService;
            _settingsService = settingsService;
        }

        private AthleteSummary _athlete;
        public AthleteSummary Athlete
        {
            get { return _athlete; }
            set { Set(() => Athlete, ref _athlete, value); }
        }

        private ObservableCollection<AthleteSummary> _followers = new ObservableCollection<AthleteSummary>();
        public ObservableCollection<AthleteSummary> Followers
        {
            get { return _followers; }
            set { Set(() => Followers, ref _followers, value); }
        }

        private ObservableCollection<AthleteSummary> _friends = new ObservableCollection<AthleteSummary>();
        public ObservableCollection<AthleteSummary> Friends
        {
            get { return _friends; }
            set { Set(() => Friends, ref _friends, value); }
        }

        private ObservableCollection<AthleteSummary> _bothFollowing = new ObservableCollection<AthleteSummary>();
        public ObservableCollection<AthleteSummary> BothFollowing
        {
            get { return _bothFollowing; }
            set { Set(() => BothFollowing, ref _bothFollowing, value); }
        }

        private ObservableCollection<SegmentEffort> _koms = new ObservableCollection<SegmentEffort>();
        public ObservableCollection<SegmentEffort> Koms
        {
            get { return _koms; }
            set { Set(() => Koms, ref _koms, value); }
        }

        private ObservableCollection<SegmentSummary> _starredSegments = new ObservableCollection<SegmentSummary>();
        public ObservableCollection<SegmentSummary> StarredSegments
        {
            get { return _starredSegments; }
            set { Set(() => StarredSegments, ref _starredSegments, value); }
        }  

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(
            () => ViewLoaded()));

        private RelayCommand<ItemClickEventArgs> _athleteTappedCommand;
        public RelayCommand<ItemClickEventArgs> AthleteTappedCommand => _athleteTappedCommand ?? (_athleteTappedCommand = new RelayCommand<ItemClickEventArgs>(OnAthleteTapped));

        private Task ViewLoaded()
        {
            if(string.IsNullOrEmpty(_currentAthleteId) || !string.Equals(_currentAthleteId, NavigationService.CurrentParameter?.ToString()))
                return LoadAsync();
            return Task.CompletedTask;
        }

        private async Task LoadAsync()
        {
            // clear old value // TODO configure ioc to give a new VM per call
            ClearProperties();
            
            string currentParameter = (string)NavigationService.CurrentParameter;
            bool authenticatedUser = string.IsNullOrEmpty(currentParameter);
            if (authenticatedUser)
            {
                Athlete = await _stravaService.GetAthleteAsync();
                await GetStarredSegmentsAsync();
            }
            else
            {
                Athlete = await _stravaService.GetAthleteAsync(currentParameter);
                await GetStarredSegmentsAsync(Athlete.Id.ToString());
            }

            if (Athlete != null)
            {
                List<Task> tasks = new List<Task>();

                _currentAthleteId = Athlete.Id.ToString();

                tasks.Add(GetFollowersAsync(_currentAthleteId, authenticatedUser));
                tasks.Add(GetFriendsAsync(_currentAthleteId, authenticatedUser));
                if(!string.IsNullOrEmpty(currentParameter))
                    tasks.Add(GetMutualFriendsAsync(_currentAthleteId));
                tasks.Add(GetKomsAsync(_currentAthleteId));

                await Task.WhenAll(tasks);
            }
        }

        private void ClearProperties()
        {
            Athlete = null;
            StarredSegments.Clear();
            Followers.Clear();
            Friends.Clear();
            BothFollowing.Clear();
            Koms.Clear();
        }

        private async Task GetFollowersAsync(string athleteId, bool authenticatedUser = true)
        {
            var followers = await _stravaService.GetFollowersAsync(athleteId, authenticatedUser);
            if (followers != null)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foreach (AthleteSummary follower in followers)
                        Followers.Add(follower);
                });
            }
        }

        private async Task GetFriendsAsync(string athleteId, bool authenticatedUser = true)
        {
            var friends = await _stravaService.GetFriendsAsync(athleteId, authenticatedUser);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                foreach (AthleteSummary friend in friends)
                    Friends.Add(friend);
            });
        }

        private async Task GetMutualFriendsAsync(string athleteId)
        {
            var friends = await _stravaService.GetMutualFriendsAsync(athleteId);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                foreach (AthleteSummary friend in friends)
                    BothFollowing.Add(friend);
            });
        }

        private async Task GetKomsAsync(string athleteId)
        {
            var koms = await _stravaService.GetKomsAsync(athleteId);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                foreach (SegmentEffort kom in koms)
                    Koms.Add(kom);
            });
        }

        private async Task GetStarredSegmentsAsync(string athleteId = null)
        {
            List<SegmentSummary> segments;
            if (string.IsNullOrEmpty(athleteId))
                segments = await _stravaService.GetStarredSegmentsAsync();
            else
                segments = await _stravaService.GetStarredSegmentsAsync(athleteId);

            if (segments != null)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    foreach (SegmentSummary segment in segments)
                        StarredSegments.Add(segment);
                });
            }
        }
    }
}
