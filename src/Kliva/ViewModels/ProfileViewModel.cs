using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

        public ProfileViewModel(INavigationService navigationService, IStravaService stravaService, ISettingsService settingsService)
            : base(navigationService)
        {
            _stravaService = stravaService;
            _settingsService = settingsService;
            _navigationService = navigationService;
        }

        private Athlete _athlete;
        public Athlete Athlete
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

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(ViewLoaded));

        private async void ViewLoaded()
        {
            // clear old value // TODO configure ioc to give a new VM per call
            ClearProperties();
            
            string currentParameter = (string)_navigationService.CurrentParameter;
            bool authenticatedUser = string.IsNullOrEmpty(currentParameter);
            if (authenticatedUser)
            {
                Athlete = await _stravaService.GetAthleteAsync();
            }
            else
            {
                Athlete = await _stravaService.GetAthleteAsync(currentParameter);
            }

            if (Athlete != null)
            {
                List<Task> tasks = new List<Task>();

                if (Athlete.FollowerCount > 0)
                    tasks.Add(GetFollowersAsync(Athlete.Id.ToString(), authenticatedUser));
                if (Athlete.FriendCount > 0)
                    tasks.Add(GetFriendsAsync(Athlete.Id.ToString(), authenticatedUser));
                if(!string.IsNullOrEmpty(currentParameter))
                    tasks.Add(GetMutualFriendsAsync(Athlete.Id.ToString()));
                tasks.Add(GetKomsAsync(Athlete.Id.ToString()));

                await Task.WhenAll(tasks);
            }
        }

        private void ClearProperties()
        {
            Athlete = null;
            Followers.Clear();
            Friends.Clear();
            BothFollowing.Clear();
            Koms.Clear();
        }

        private async Task GetFollowersAsync(string athleteId, bool authenticatedUser = true)
        {
            var followers = await _stravaService.GetFollowersAsync(athleteId, authenticatedUser);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                foreach (AthleteSummary follower in followers)
                    Followers.Add(follower);
            });
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
            var defaultUnit = await _settingsService.GetStoredDistanceUnitTypeAsync();
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                foreach (SegmentEffort kom in koms)
                {
                    // TODO kom.FormatFields(defaultUnit);
                    Koms.Add(kom);
                }
            });
        }
    }
}
