using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Kliva.Helpers;
using Kliva.Messages;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Kliva.Views;
using Microsoft.Practices.ServiceLocation;

namespace Kliva.ViewModels
{
    public class SegmentViewModel : KlivaBaseViewModel
    {
        private readonly IStravaService _stravaService;

        private string _currentSegmentId;

        public Segment Segment { get; set; }

        private SegmentEffort _segmentEffort;
        public SegmentEffort SegmentEffort
        {
            get { return _segmentEffort;}
            set { Set(() => SegmentEffort, ref _segmentEffort, value); }
        }

        private Leaderboard _leaderboardOverall;
        public Leaderboard LeaderboardOverall
        {
            get { return _leaderboardOverall; }
            set { Set(() => LeaderboardOverall, ref _leaderboardOverall, value); }
        }

        private Leaderboard _leaderboardFollowing;
        public Leaderboard LeaderboardFollowing
        {
            get { return _leaderboardFollowing; }
            set { Set(() => LeaderboardFollowing, ref _leaderboardFollowing, value); }
        }

        private ObservableCollection<Grouping<string, LeaderboardEntry>> _groupedLeaderboards = new ObservableCollection<Grouping<string, LeaderboardEntry>>();
        public ObservableCollection<Grouping<string, LeaderboardEntry>> GroupedLeaderboards
        {
            get { return _groupedLeaderboards; }
            set { Set(() => GroupedLeaderboards, ref _groupedLeaderboards, value); }
        }

        private RelayCommand _viewLoadedCommand;       
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(() => ViewLoaded()));

        private RelayCommand _mapCommand;
        public RelayCommand MapCommand => _mapCommand ?? (_mapCommand = new RelayCommand(() => NavigationService.Navigate<MapPage>(Segment?.Map)));

        public SegmentViewModel(INavigationService navigationService, IStravaService stravaService) : base(navigationService)
        {
            _stravaService = stravaService;
        }

        private Task ViewLoaded()
        {
            if (string.IsNullOrEmpty(_currentSegmentId) || !string.Equals(_currentSegmentId, NavigationService.CurrentParameter?.ToString()))
                return LoadAsync();

            return Task.CompletedTask;
        }

        private async Task LoadAsync()
        {
            //TODO: All - configure ioc to give a new VM per call?
            ClearProperties();

            string currentParameter = (string)NavigationService.CurrentParameter;
            if (!string.IsNullOrEmpty(currentParameter))
            {
                //TODO: Glenn - What do we need? Segment or Segment Effort or both?
                //TODO: Glenn - We need Segment Effort for analytics! So move analytics groups to SegmentEffortClass
                SegmentEffort = await _stravaService.GetSegmentEffortAsync(currentParameter);
                //TODO: Glenn - We need to retrieve the actual segment too, for extra data ( like MAP ) - Look how we can combine/merge this with SegmentEffort.Segment
                Segment = await _stravaService.GetSegmentAsync(SegmentEffort.Segment.Id.ToString());

                LeaderboardOverall = await _stravaService.GetLeaderboardOverallAsync(SegmentEffort.Segment.Id.ToString());
                LeaderboardFollowing = await _stravaService.GetLeaderboardFollowingAsync(SegmentEffort.Segment.Id.ToString());

                GroupedLeaderboards.Clear();
                IEnumerable<LeaderboardEntry> overallEntries = LeaderboardOverall?.Entries.Take(10);
                if (overallEntries != null)
                {
                    //TODO: Glenn - Use translation!
                    Grouping<string, LeaderboardEntry> grouped = new Grouping<string, LeaderboardEntry>("overall", overallEntries);
                    GroupedLeaderboards.Add(grouped);
                }

                if (LeaderboardFollowing.Entries.Any())
                {
                    //TODO: Glenn - Use translation!
                    Grouping<string, LeaderboardEntry> grouped = new Grouping<string, LeaderboardEntry>("following", LeaderboardFollowing.Entries);
                    GroupedLeaderboards.Add(grouped);
                }

                ServiceLocator.Current.GetInstance<IMessenger>()
                    .Send<PolylineMessage>(Segment.Map.GeoPositions.Any()
                        ? new PolylineMessage(Segment.Map.GeoPositions)
                        : new PolylineMessage(new List<BasicGeoposition>()), Tokens.SegmentPolylineMessage);
            }
        }

        

        private void ClearProperties()
        {
            SegmentEffort = null;
        }
    }
}
