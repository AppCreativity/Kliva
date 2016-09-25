using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kliva.Helpers;
using Kliva.Models;
using Kliva.Services.Interfaces;

namespace Kliva.Services
{
    public class StravaSegmentService : IStravaSegmentService
    {
        private readonly ISettingsService _settingsService;
        private readonly StravaWebClient _stravaWebClient;

        private readonly ConcurrentDictionary<string, Task<List<SegmentSummary>>> _cachedStarredSegmentsTasks = new ConcurrentDictionary<string, Task<List<SegmentSummary>>>();

        //TODO: Glenn - How long before we invalidate an in memory cached segment? Maybe use MemoryCache? https://msdn.microsoft.com/en-us/library/system.runtime.caching.memorycache(v=vs.110).aspx
        private readonly ConcurrentDictionary<string, Task<Segment>> _cachedSegmentTasks = new ConcurrentDictionary<string, Task<Segment>>();
        private readonly ConcurrentDictionary<string, Task<SegmentEffort>> _cachedSegmentEffortTasks = new ConcurrentDictionary<string, Task<SegmentEffort>>();
        private readonly ConcurrentDictionary<string, Task<Leaderboard>> _cachedLeaderBoardTasks = new ConcurrentDictionary<string, Task<Leaderboard>>();

        public StravaSegmentService(ISettingsService settingsService, StravaWebClient stravaWebClient)
        {
            _settingsService = settingsService;
            _stravaWebClient = stravaWebClient;
        }

        private static void FillStatistics(SegmentEffort segmentEffort)
        {
            StatisticsGroup current = new StatisticsGroup() { Name = "this effort", Sort = 0, Type = StatisticGroupType.Current};
            StatisticsDetail movingTimeCurrent = new StatisticsDetail()
            {
                Sort = 0,
                Icon = "",
                DisplayDescription = "moving time",
                DisplayValue = $"{Helpers.Converters.SecToTimeConverter.Convert(segmentEffort.ElapsedTime, typeof(int), null, string.Empty)}",
                Group = current
            };

            StatisticsDetail averageSpeedCurrent = new UserMeasurementUnitStatisticsDetail(segmentEffort.AverageSpeedMeasurementUnit)
            {
                Sort = 1,
                Icon = "",
                DisplayDescription = "average speed",
                Group = current
            };

            StatisticsDetail averageHeartRateCurrent = new StatisticsDetail()
            {
                Sort = 2,
                Icon = "",
                DisplayDescription = "average heart rate",
                DisplayValue = $"{segmentEffort.AverageHeartrate} bpm",
                Group = current
            };

            StatisticsDetail maxHeartRateCurrent = new StatisticsDetail()
            {
                Sort = 3,
                Icon = "",
                DisplayDescription = "max heart rate",
                DisplayValue = $"{segmentEffort.MaxHeartrate} bpm",
                Group = current
            };

            current.Details.Add(movingTimeCurrent);
            current.Details.Add(averageSpeedCurrent);
            current.Details.Add(averageHeartRateCurrent);
            current.Details.Add(maxHeartRateCurrent);

            segmentEffort.Statistics.Add(current);
        }

        private async Task<Segment> GetSegmentFromServiceAsync(string segmentId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();

                string getUrl = $"{Endpoints.Segment}/{segmentId}?access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                var segment = Unmarshaller<Segment>.Unmarshal(json);
                StravaService.SetMetricUnits(segment, defaultDistanceUnitType);

                return segment;
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }

        private async Task<SegmentEffort> GetSegmentEffortFromServiceAsync(string segmentEffortId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();

                string getUrl = $"{Endpoints.SegmentEffort}/{segmentEffortId}?access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                var segmentEffort = Unmarshaller<SegmentEffort>.Unmarshal(json);
                StravaService.SetMetricUnits(segmentEffort, defaultDistanceUnitType);

                FillStatistics(segmentEffort);

                return segmentEffort;
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }

        private async Task<Leaderboard> GetLeaderBoardFromServiceAsync(string segmentId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();

                string getUrl = $"{string.Format(Endpoints.Leaderboard, segmentId)}?access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                var leaderboard = Unmarshaller<Leaderboard>.Unmarshal(json);
                if (leaderboard.Entries != null)
                {
                    foreach (LeaderboardEntry entry in leaderboard.Entries)
                        StravaService.SetMetricUnits(entry, defaultDistanceUnitType);
                }

                return leaderboard;
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }

        /// <summary>
        /// Gets all the starred segments of an Athlete.
        /// </summary>
        /// <returns>A list of segments that are starred by the athlete.</returns>
        private async Task<List<SegmentSummary>> GetStarredSegmentsFromServiceAsync(string athleteId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();

                string getUrl = $"{Endpoints.Athletes}/{athleteId}/segments/starred?access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                var segments = Unmarshaller<List<SegmentSummary>>.Unmarshal(json);
                foreach (SegmentSummary segment in segments)
                    StravaService.SetMetricUnits(segment, defaultDistanceUnitType);

                return segments;
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }

        public void FillStatistics(SegmentEffort segmentEffort, Leaderboard leaderboard)
        {
            if (leaderboard != null)
            {
                var entry = (from element in leaderboard.Entries
                             where element.AthleteId == segmentEffort.Athlete.Id
                             select element).FirstOrDefault();

                if (entry != null)
                {
                    //TODO: Glenn - Verify SegmentViewModel - There we also retrieve the corresponding Segment for MAP info, maybe better we do it here in the Service?? ( Merge/Combine )
                    entry.Segment = segmentEffort.Segment;

                    StatisticsGroup pr = new StatisticsGroup() {Name = "personal record", Sort = 1, Type = StatisticGroupType.PR};
                    StatisticsDetail movingTimePR = new StatisticsDetail()
                    {
                        Sort = 0,
                        Icon = "",
                        DisplayDescription = "moving time",
                        DisplayValue =
                            $"{Helpers.Converters.SecToTimeConverter.Convert(entry.MovingTime, typeof (int), null, string.Empty)}",
                        Group = pr
                    };

                    StatisticsDetail averageSpeedPR = new UserMeasurementUnitStatisticsDetail(segmentEffort.AverageSpeedMeasurementUnit)
                    {
                        Sort = 1,
                        Icon = "",
                        DisplayDescription = "average speed",
                        Group = pr
                    };

                    StatisticsDetail averageHeartRatePR = new StatisticsDetail()
                    {
                        Sort = 2,
                        Icon = "",
                        DisplayDescription = "average heart rate",
                        DisplayValue = $"{entry.AverageHeartrateDisplay} bpm",
                        Group = pr
                    };

                    StatisticsDetail rankPR = new StatisticsDetail()
                    {
                        Sort = 2,
                        Icon = "",
                        DisplayDescription = "rank",
                        DisplayValue = $"{entry.Rank}/{leaderboard.EntryCount}",
                        Group = pr
                    };

                    pr.Details.Add(movingTimePR);
                    pr.Details.Add(averageSpeedPR);
                    pr.Details.Add(averageHeartRatePR);
                    pr.Details.Add(rankPR);

                    segmentEffort.Statistics.Add(pr);
                }
            }
        }

        public Task<Segment> GetSegmentAsync(string segmentId)
        {
            return _cachedSegmentTasks.GetOrAdd(segmentId, GetSegmentFromServiceAsync);
        }

        public Task<SegmentEffort> GetSegmentEffortAsync(string segmentEffortId)
        {
            return _cachedSegmentEffortTasks.GetOrAdd(segmentEffortId, GetSegmentEffortFromServiceAsync);
        }

        public async Task<List<SegmentSummary>> GetStarredSegmentsAsync()
        {
            //TODO: Glenn - Caching?
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();

                string getUrl = $"{Endpoints.Starred}?access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                var segments = Unmarshaller<List<SegmentSummary>>.Unmarshal(json);
                foreach (SegmentSummary segment in segments)
                    StravaService.SetMetricUnits(segment, defaultDistanceUnitType);

                return segments;
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }

        public Task<List<SegmentSummary>> GetStarredSegmentsAsync(string athleteId)
        {
            return _cachedStarredSegmentsTasks.GetOrAdd(athleteId, GetStarredSegmentsFromServiceAsync);
        }

        public Task<Leaderboard> GetLeaderBoardAsync(string segmentId)
        {
            return _cachedLeaderBoardTasks.GetOrAdd(segmentId, GetLeaderBoardFromServiceAsync);
        }
    }
}
