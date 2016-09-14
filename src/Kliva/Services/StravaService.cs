using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using GalaSoft.MvvmLight.Threading;
using Kliva.Controls;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;

namespace Kliva.Services
{
    public enum StravaServiceStatus
    {
        Failed,
        Success
    }

    public class StravaServiceEventArgs : EventArgs
    {
        public StravaServiceStatus Status { get; private set; }
        public Exception Exception { get; private set; }

        public StravaServiceEventArgs(StravaServiceStatus status, Exception ex = null)
        {
            Status = status;
            Exception = ex;
        }
    }

    /// <summary>
    /// Service layer on top of the basic Strava services to be able to combine calls on different API endpoints
    /// </summary>
    public class StravaService : IStravaService
    {
        public IStravaActivityService StravaActivityService => ServiceLocator.Current.GetInstance<IStravaActivityService>();

        public IStravaAthleteService StravaAthleteService => ServiceLocator.Current.GetInstance<IStravaAthleteService>();

        public IStravaClubService StravaClubService => ServiceLocator.Current.GetInstance<IStravaClubService>();

        public IStravaSegmentService StravaSegmentService => ServiceLocator.Current.GetInstance<IStravaSegmentService>();

        private string ParseAuthorizationResponse(string responseData)
        {
            var authorizationCodeIndex = responseData.IndexOf("&code=", StringComparison.Ordinal) + 6;
            return responseData.Substring(authorizationCodeIndex, responseData.Length - authorizationCodeIndex);
        }

        private async Task GetAccessToken(string authorizationCode)
        {
            try
            {
                var values = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("client_id", StravaIdentityConstants.STRAVA_AUTHORITY_CLIENT_ID),
                        new KeyValuePair<string, string>("client_secret", StravaIdentityConstants.STRAVA_AUTHORITY_CLIENT_SECRET),
                        new KeyValuePair<string, string>("code", authorizationCode)
                    };

                var httpClient = new HttpClient(new HttpClientHandler());
                var response = await httpClient.PostAsync(Constants.STRAVA_AUTHORITY_TOKEN_URL, new FormUrlEncodedContent(values));
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();

                var accessToken = JsonConvert.DeserializeObject<AccessToken>(responseString);
                await ServiceLocator.Current.GetInstance<ISettingsService>().SetStravaAccessTokenAsync(accessToken.Token);

                OnStatusEvent(new StravaServiceEventArgs(StravaServiceStatus.Success));
            }
            catch(Exception ex)
            {
                OnStatusEvent(new StravaServiceEventArgs(StravaServiceStatus.Failed, ex));
            }
        }

        private Task GetActivitySummaryRelationsAsync(IEnumerable<ActivitySummary> activities)
        {
            foreach (var activity in activities)
            {
                var athleteTask = StravaAthleteService.GetAthleteAsync(activity.AthleteMeta.Id.ToString());
                var t = athleteTask.ContinueWith(c =>
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            activity.Athlete = c.Result;
                        });
                    });

                if (activity.AthleteMeta.Id == StravaAthleteService.Athlete.Id && activity.TotalPhotoCount > 0)
                {
                    var phototask = StravaActivityService.GetPhotosAsync(activity.Id.ToString());
                    t = phototask.ContinueWith(c =>
                        {
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                            {
                                activity.AllPhotos = c.Result;
                            });
                        });
                }
            }

            return Task.CompletedTask;
        }

        //TODO: Glenn - Should we set these at some BaseClass?
        public static void SetMetricUnits(ActivitySummary activity, DistanceUnitType distanceUnitType)
        {
            activity.DistanceUnit = distanceUnitType;
            activity.SpeedUnit = activity.DistanceUnit == DistanceUnitType.Kilometres ? SpeedUnit.KilometresPerHour : SpeedUnit.MilesPerHour;
            activity.ElevationUnit = activity.DistanceUnit == DistanceUnitType.Kilometres ? DistanceUnitType.Metres : DistanceUnitType.Feet;
        }

        //TODO: Glenn - Should we set these at some BaseClass?
        public static void SetMetricUnits(SegmentEffort segment, DistanceUnitType distanceUnitType)
        {
            segment.DistanceUnit = distanceUnitType;
            segment.SpeedUnit = segment.DistanceUnit == DistanceUnitType.Kilometres ? SpeedUnit.KilometresPerHour : SpeedUnit.MilesPerHour;
            segment.ElevationUnit = segment.DistanceUnit == DistanceUnitType.Kilometres ? DistanceUnitType.Metres : DistanceUnitType.Feet;
        }

        //TODO: Glenn - Should we set these at some BaseClass?
        public static void SetMetricUnits(SegmentSummary segment, DistanceUnitType distanceUnitType)
        {
            segment.DistanceUnit = distanceUnitType;
            segment.SpeedUnit = segment.DistanceUnit == DistanceUnitType.Kilometres ? SpeedUnit.KilometresPerHour : SpeedUnit.MilesPerHour;
            segment.ElevationUnit = segment.DistanceUnit == DistanceUnitType.Kilometres ? DistanceUnitType.Metres : DistanceUnitType.Feet;
        }

        //TODO: Glenn - Should we set these at some BaseClass?
        public static void SetMetricUnits(LeaderboardEntry entry, DistanceUnitType distanceUnitType)
        {
            entry.DistanceUnit = distanceUnitType;
            entry.SpeedUnit = entry.DistanceUnit == DistanceUnitType.Kilometres ? SpeedUnit.KilometresPerHour : SpeedUnit.MilesPerHour;
            entry.ElevationUnit = entry.DistanceUnit == DistanceUnitType.Kilometres ? DistanceUnitType.Metres : DistanceUnitType.Feet;
        }

        #region Event handlers
        public event EventHandler<StravaServiceEventArgs> StatusEvent;

        protected virtual void OnStatusEvent(StravaServiceEventArgs e)
        {
            StatusEvent?.Invoke(this, e);
        }
        #endregion

        public async Task GetAuthorizationCode()
        {
            string authenticationURL = string.Format("{0}?client_id={1}&response_type=code&redirect_uri={2}&scope=write&state=mystate&approval_prompt=force", Constants.STRAVA_AUTHORITY_AUTHORIZE_URL, StravaIdentityConstants.STRAVA_AUTHORITY_CLIENT_ID, Constants.STRAVA_AUTHORITY_REDIRECT_URL);

            try
            {
                WebAuthenticationResult webAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, new Uri(authenticationURL), new Uri(Constants.STRAVA_AUTHORITY_REDIRECT_URL));                

                if (webAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    var responseData = webAuthenticationResult.ResponseData;
                    var tempAuthorizationCode = ParseAuthorizationResponse(responseData);
                    await GetAccessToken(tempAuthorizationCode);
                }
            }
            catch(Exception ex)
            {
                OnStatusEvent(new StravaServiceEventArgs(StravaServiceStatus.Failed, ex));
            }
        }

        /// <summary>
        /// Get authenticated athlete
        /// </summary>
        /// <returns></returns>
        public Task<Athlete> GetAthleteAsync()
        {
            return StravaAthleteService.GetAthleteAsync();
        }

        /// <summary>
        /// Get a non-authenticated athlete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<AthleteSummary> GetAthleteAsync(string id)
        {
            return StravaAthleteService.GetAthleteAsync(id);
        }

        public Task<IEnumerable<AthleteSummary>> GetFollowersAsync(string athleteId, bool authenticatedUser = true)
        {
            return StravaAthleteService.GetFollowersAsync(athleteId, authenticatedUser);
        }

        public Task<IEnumerable<AthleteSummary>> GetFriendsAsync(string athleteId, bool authenticatedUser = true)
        {
            return StravaAthleteService.GetFriendsAsync(athleteId, authenticatedUser);
        }

        public Task<IEnumerable<AthleteSummary>> GetMutualFriendsAsync(string athleteId)
        {
            return StravaAthleteService.GetMutualFriendsAsync(athleteId);
        }

        public Task<IEnumerable<SegmentEffort>> GetKomsAsync(string athleteId)
        {
            return StravaAthleteService.GetKomsAsync(athleteId);
        }

        public async Task<Activity> GetActivityAsync(string id, bool includeEfforts)
        {
            //TODO: Glenn - kick of tasks in Task.Run List<Task>
            Activity activity = await StravaActivityService.GetActivityAsync(id, includeEfforts);

            if (activity != null)
            {
                await GetActivitySummaryRelationsAsync(new List<ActivitySummary> { activity });

                if (activity.OtherAthleteCount > 0)
                {
                    activity.RelatedActivities = await StravaActivityService.GetRelatedActivitiesAsync(id);
                    await GetActivitySummaryRelationsAsync(activity.RelatedActivities);
                }

                if (activity.KudosCount > 0)
                    activity.Kudos = await StravaActivityService.GetKudosAsync(id);

                if (activity.CommentCount > 0)
                    activity.Comments = await StravaActivityService.GetCommentsAsync(id);
                }

            return activity;
        }

        public Task<string> GetFriendActivityDataAsync(int page, int pageSize)
        {
            return StravaActivityService.GetFriendActivityDataAsync(page, pageSize);
        }

        public Task<string> GetMyActivityDataAsync(int page, int pageSize)
        {
            return StravaActivityService.GetMyActivityDataAsync(page, pageSize);
        }

        public async Task<List<ActivitySummary>> HydrateActivityData(string data)
        {
            var activities = await StravaActivityService.HydrateActivityData(data);
            if (activities != null && activities.Any())
                await GetActivitySummaryRelationsAsync(activities);

            return activities;
        }

        public Task GiveKudosAsync(string activityId)
        {
            return StravaActivityService.GiveKudosAsync(activityId);
        }

        public Task PostComment(string activityId, string text)
        {
            return StravaActivityService.PostComment(activityId, text);
        }

        public Task PutUpdate(string activityId, string name, bool commute, bool isPrivate, string gearID)
        {
            return StravaActivityService.PutUpdate(activityId, name, commute, isPrivate, gearID);
        }

        public Task UploadActivityAsync(string gpxFilePath, ActivityType activityType, string name, bool commute, bool isPrivate)
        {
            return StravaActivityService.UploadActivityAsync(gpxFilePath, activityType, name, commute, isPrivate);
        }


        public Task<List<ClubSummary>> GetClubsAsync()
        {
            return StravaClubService.GetClubsAsync();
        }

        public async Task<Club> GetClubAsync(string id)
        {
            //TODO: Glenn - kick of tasks in Task.Run List<Task>

            Club club = await StravaClubService.GetClubAsync(id);
            if (club != null)
            {
                if (club.MemberCount > 0)
                    club.Members = await StravaClubService.GetClubMembersAsync(id);
            }

            return club;
        }

        public Task<Segment> GetSegmentAsync(string segmentId)
        {
            return StravaSegmentService.GetSegmentAsync(segmentId);
        }

        public async Task<SegmentEffort> GetSegmentEffortAsync(string segmentEffortId)
        {
            SegmentEffort segmentEffort = await StravaSegmentService.GetSegmentEffortAsync(segmentEffortId);

            if (segmentEffort != null)
            {
                //TODO: Glenn - Load up Leaderboard to show PR in SegmentEffort!
                Leaderboard leaderboard = await StravaSegmentService.GetLeaderBoardAsync(segmentEffort.Segment.Id.ToString());

                if (leaderboard != null && segmentEffort.Statistics.FirstOrDefault(item => item.Type == StatisticGroupType.PR) == null)
                    StravaSegmentService.FillStatistics(segmentEffort, leaderboard);
            }

            return segmentEffort;
        }

        public Task<List<SegmentSummary>> GetStarredSegmentsAsync()
        {
            return StravaSegmentService.GetStarredSegmentsAsync();
        }

        public Task<List<SegmentSummary>> GetStarredSegmentsAsync(string athleteId)
        {
            return StravaSegmentService.GetStarredSegmentsAsync(athleteId);
        }

        public AthleteSummary ConsolidateWithCache(AthleteMeta athlete)
        {
            return StravaAthleteService.ConsolidateWithCache(athlete);
        }
    }
}