using Kliva.Helpers;
using Kliva.Models;
using Kliva.Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Kliva.Services.Performance;
using Microsoft.Practices.ServiceLocation;

namespace Kliva.Services
{
    public class StravaActivityService : IStravaActivityService
    {
        private readonly ISettingsService _settingsService;
        private readonly ILogService _logService;
        private readonly StravaWebClient _stravaWebClient;

        private readonly ETWLogging _perflog;

        //TODO: Glenn - When to Invalidate cache?
        private readonly ConcurrentDictionary<string, Task<List<Photo>>> _cachedPhotosTasks = new ConcurrentDictionary<string, Task<List<Photo>>>();

        //TODO: Glenn - When to Invalidate cache?
        private readonly ConcurrentDictionary<string, Task<IList<ActivitySummary>>> _cachedRelatedActivitiesTasks = new ConcurrentDictionary<string, Task<IList<ActivitySummary>>>();        

        public StravaActivityService(ISettingsService settingsService, ILogService logService, StravaWebClient stravaWebClient)
        {
            _settingsService = settingsService;
            _logService = logService;
            _stravaWebClient = stravaWebClient;

            _perflog = ETWLogging.Log;
        }

        private async Task<List<Photo>> GetPhotosFromServiceAsync(string activityId)
        {
            try
            {
                _perflog.GetPhotosFromService(false, activityId);

                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();

                string getUrl = $"{Endpoints.Activity}/{activityId}/photos?photo_sources=true&size=600&access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                var results = Unmarshaller<List<Photo>>.Unmarshal(json);
                _perflog.GetPhotosFromService(true, activityId);

                return results;
            }
            catch (Exception ex)
            {
                string title = $"StravaActivityService.GetPhotosFromServiceAsync - activityId {activityId}";
                _logService.LogException(title, ex);
            }

            return null;
        }

        private async Task<IList<ActivitySummary>> GetRelatedActivitiesFromServiceAsync(string activityId)
        {
            try
            {
                _perflog.GetRelatedActivitiesFromService(false, activityId);

                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();

                string getUrl = $"{Endpoints.Activity}/{activityId}/related?access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                var results = Unmarshaller<List<ActivitySummary>>.Unmarshal(json).Select(activity =>
                {
                    StravaService.SetMetricUnits(activity, defaultDistanceUnitType);
                    return activity;
                }).ToList();

                _perflog.GetRelatedActivitiesFromService(true, activityId);
                return results;
            }
            catch (Exception ex)
            {
                string title = $"StravaActivityService.GetRelatedActivitiesFromServiceAsync - activityId {activityId}";
                _logService.LogException(title, ex);
            }

            return null;
        }

        private static void FillStatistics(Activity activity)
        {
            var distance = StatisticsHelper.CreateGroup("distance", 0, "", "total distance", 
                activity.DistanceUserMeasurementUnit).Details[0];

            var speedGroup= StatisticsHelper.CreateGroup("speed", 1, "", "average speed",
                activity.AverageSpeedUserMeasurementUnit);
            StatisticsHelper.CreateDetailForGroup(speedGroup, 1, "", "max speed", 
                activity.MaxSpeedUserMeasurementUnit);

            var movingTime = StatisticsHelper.CreateGroup("time", 2, "", "moving time",
                $"{Helpers.Converters.SecToTimeConverter.Convert(activity.MovingTime, typeof(int), null, string.Empty)}").Details[0];

            var elevationGain = StatisticsHelper.CreateGroup("elevation", 3, "", "elevation gain",
                activity.ElevationGainUserMeasurementUnit).Details[0];

            var heartRateGroup = StatisticsHelper.CreateGroup("heart rate", 4, "", "average heart rate",
                $"{Math.Round(activity.AverageHeartrate)} bpm");
            StatisticsHelper.CreateDetailForGroup(heartRateGroup, 1, "", "max heart rate",
                $"{activity.MaxHeartrate} bpm");

            activity.Statistics.Add(distance);
            activity.Statistics.Add(speedGroup.Details[0]);
            activity.Statistics.Add(speedGroup.Details[1]);
            activity.Statistics.Add(movingTime);
            activity.Statistics.Add(elevationGain);
            activity.Statistics.Add(heartRateGroup.Details[0]);
            activity.Statistics.Add(heartRateGroup.Details[1]);
        }

        /// <summary>
        /// Gets a single activity from Strava asynchronously.
        /// </summary>
        /// <param name="id">The Strava activity id.</param>
        /// <param name="includeEfforts">Used to include all segment efforts in the result.</param>
        /// <returns>The activity with the specified id.</returns>
        public async Task<Activity> GetActivityAsync(string id, bool includeEfforts)
        {
            try
            {
                _perflog.GetActivityAsync(false, id, includeEfforts);

                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();

                string getUrl = $"{Endpoints.Activity}/{id}?include_all_efforts={includeEfforts}&access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                if (!string.IsNullOrEmpty(json))
                {
                    var activity = Unmarshaller<Activity>.Unmarshal(json);
                    StravaService.SetMetricUnits(activity, defaultDistanceUnitType);
                    if (activity.SegmentEfforts != null)
                    {
                        foreach (SegmentEffort segment in activity.SegmentEfforts)
                            StravaService.SetMetricUnits(segment, defaultDistanceUnitType);
                    }

                    FillStatistics(activity);

                    _perflog.GetActivityAsync(true, id, includeEfforts);

                    string activityUri = $"{Endpoints.PublicActivity}/{id}";
                    _logService.Log("API", "GetActivityAsync", activityUri);

                    return activity;
                }
                else
                    throw new NullReferenceException("Strava returned no Json data for requested activity");
            }
            catch (Exception ex)
            {
                string title = $"StravaActivityService.GetActivityAsync - id {id} - includeEfforts {includeEfforts}";
                _logService.LogException(title, ex);
            }

            return null;
        }

        /// <summary>
        /// Gets all the activities asynchronously. Pagination is supported.
        /// </summary>
        /// <param name="page">The page of activities.</param>
        /// <param name="perPage">The amount of activities that are loaded per page.</param>
        /// <returns>A list of activities.</returns>
        public async Task<IList<ActivitySummary>> GetActivitiesAsync(int page, int perPage)
        {
            try
            {
                _perflog.GetActivitiesAsync(false, page, perPage);
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();

                //TODO: Glenn - Optional parameters should be treated as such!
                string getUrl = $"{Endpoints.Activities}?page={page}&per_page={perPage}&access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                var results = Unmarshaller<List<ActivitySummary>>.Unmarshal(json).Select(activity =>
                {
                    StravaService.SetMetricUnits(activity, defaultDistanceUnitType);
                    return activity;
                }).ToList();
                _perflog.GetActivitiesAsync(true, page, perPage);
                return results;
            }
            catch (Exception ex)
            {
                string title = $"StravaActivityService.GetActivitiesAsync - page {page} - perPage {perPage}";
                _logService.LogException(title, ex);
            }

            return null;
        }

        /// <summary>
        /// Gets the latest activities of the currently authenticated athletes followers asynchronously.
        /// </summary>
        /// <param name="page">The page of activities.</param>
        /// <param name="perPage">The amount of activities per page.</param>
        /// <returns>A list of activities from your followers.</returns>
        public async Task<IList<ActivitySummary>> GetFollowersActivitiesAsync(int page, int perPage)
        {
            try
            {
                _perflog.GetFollowersActivitiesAsync(false, page, perPage);
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();

                //TODO: Glenn - Optional parameters should be treated as such!
                string getUrl = $"{Endpoints.ActivitiesFollowers}?page={page}&per_page={perPage}&access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                var results = Unmarshaller<List<ActivitySummary>>.Unmarshal(json).Select(activity =>
                {
                    StravaService.SetMetricUnits(activity, defaultDistanceUnitType);
                    return activity;
                }).ToList();
                _perflog.GetFollowersActivitiesAsync(true, page, perPage);
                return results;
            }
            catch (Exception ex)
            {
                string title = $"StravaActivityService.GetFollowersActivitiesAsync - page {page} - perPage {perPage}";
                _logService.LogException(title, ex);
            }

            return null;
        }

        public Task<IList<ActivitySummary>> GetRelatedActivitiesAsync(string activityId)
        {
            return _cachedRelatedActivitiesTasks.GetOrAdd(activityId, GetRelatedActivitiesFromServiceAsync);
        }

        /// <summary>
        /// Gets a list of athletes that kudoed the specified activity asynchronously.
        /// </summary>
        /// <param name="activityId">The Strava Id of the activity.</param>
        /// <returns>A list of athletes that kudoed the specified activity.</returns>
        public async Task<List<AthleteSummary>> GetKudosAsync(string activityId)
        {
            try
            {
                _perflog.GetKudosAsync(false, activityId);
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();

                string getUrl = $"{Endpoints.Activity}/{activityId}/kudos?access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                var results = Unmarshaller<List<AthleteSummary>>.Unmarshal(json);
                _perflog.GetKudosAsync(true, activityId);
                return results;
            }
            catch (Exception ex)
            {
                string title = $"StravaActivityService.GetKudosAsync - activityId {activityId}";
                _logService.LogException(title, ex);
            }

            return null;
        }

        /// <summary>
        /// Give kudos for the specified activity.
        /// </summary>
        /// <param name="activityId">The activity you want to give kudos for.</param>
        public async Task GiveKudosAsync(string activityId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();

                string postUrl = $"{Endpoints.Activity}/{activityId}/kudos?access_token={accessToken}";
                await _stravaWebClient.SendPostAsync(new Uri(postUrl));
            }
            catch (Exception ex)
            {
                string title = $"StravaActivityService.GiveKudosAsync - activityId {activityId}";
                _logService.LogException(title, ex);
            }
        }

        /// <summary>
        /// Gets all the comments of an activity asynchronously.
        /// </summary>
        /// <param name="activityId">The Strava Id of the activity.</param>
        /// <returns>A list of comments.</returns>
        public async Task<List<Comment>> GetCommentsAsync(string activityId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();

                string getUrl = $"{Endpoints.Activity}/{activityId}/comments?access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                return Unmarshaller<List<Comment>>.Unmarshal(json);
            }
            catch (Exception ex)
            {
                string title = $"StravaActivityService.GetCommentsAsync - activityId {activityId}";
                _logService.LogException(title, ex);
            }

            return null;
        }

        /// <summary>
        /// Post a comment on the specified activity.
        /// </summary>
        /// <param name="activityId">The Strava ID of the activity you want to comment.</param>
        /// <param name="text">The text that will be posted.</param>
        public async Task PostComment(string activityId, string text)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                string postUrl = $"{Endpoints.Activity}/{activityId}/comments?text={WebUtility.UrlEncode(text)}&access_token={accessToken}";
                await _stravaWebClient.SendPostAsync(new Uri(postUrl));
            }
            catch (Exception ex)
            {
                string title = $"StravaActivityService.PostComment - activityId {activityId} - text {text}";
                _logService.LogException(title, ex);
            }
        }

        /// <summary>
        /// Edit a given activity
        /// </summary>
        /// <param name="activityId">The Strava ID of the activity you want to edit.</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task PutUpdate(string activityId, string name, bool commute, bool isPrivate, string gearID)
        {
            try
            {
                int commuteAsInt = commute ? 1 : 0;
                int isPrivateAsInt = isPrivate ? 1 : 0;

                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                string postUrl = $"{Endpoints.Activity}/{activityId}?name={WebUtility.UrlEncode(name)}&commute={commuteAsInt}&private={isPrivateAsInt}";
                if (!string.IsNullOrEmpty(gearID))
                    postUrl += $"&gear_id={WebUtility.UrlEncode(gearID)}";

                postUrl += $"&access_token={accessToken}";

                await _stravaWebClient.SendPutAsync(new Uri(postUrl));
            }
            catch (Exception ex)
            {
                string title = $"StravaActivityService.PutUpdate - activityId {activityId} - name {name} - commute {commute} - isPrivate {isPrivate} - gearID {gearID}";
                _logService.LogException(title, ex);
            }
        }

        /// <summary>
        /// Returns a list of photos linked to the specified activity.
        /// </summary>
        /// <param name="activityId">The activity</param>
        /// <returns>A list of photos.</returns>
        public Task<List<Photo>> GetPhotosAsync(string activityId)
        {
            return _cachedPhotosTasks.GetOrAdd(activityId, GetPhotosFromServiceAsync);
        }

        public async Task<string> GetFriendActivityDataAsync(int page, int perPage)
        {
            string data = null;

            try
            {
                _perflog.GetFriendActivityDataAsync(false, page, perPage);
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();

                //TODO: Glenn - Optional parameters should be treated as such!
                string getUrl = $"{Endpoints.ActivitiesFollowers}?page={page}&per_page={perPage}&access_token={accessToken}";
                data = await _stravaWebClient.GetAsync(new Uri(getUrl));
            }
            catch (Exception ex)
            {
                string title = $"StravaActivityService.GetFriendActivityDataAsync - page {page} - perPage {perPage}";
                _logService.LogException(title, ex);
            }

            _perflog.GetFriendActivityDataAsync(true, page, perPage);
            return data;
        }

        public async Task<string> GetMyActivityDataAsync(int page, int perPage)
        {
            string data = null;

            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                string getUrl = $"{Endpoints.Activities}?page={page}&per_page={perPage}&access_token={accessToken}";
                data = await _stravaWebClient.GetAsync(new Uri(getUrl));
            }
            catch (Exception ex)
            {
                string title = $"StravaActivityService.GetMyActivityDataAsync - page {page} - perPage {perPage}";
                _logService.LogException(title, ex);
            }
            return data;
        }

        public async Task UploadActivityAsync(string gpxFilePath, ActivityType activityType, string name, bool commute = false, bool isPrivate = false)
        {
            try
            {
                int commuteAsInt = commute ? 1 : 0;
                int isPrivateAsInt = isPrivate ? 1 : 0;

                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                string postUrl = $"{Endpoints.Uploads}?data_type=gpx&activity_type={WebUtility.UrlEncode(activityType.ToString().ToLower())}&commute={commuteAsInt}&private={isPrivateAsInt}";

                if(!string.IsNullOrEmpty(name))
                    postUrl += $"&name={WebUtility.UrlEncode(name)}";

                postUrl += $"&access_token={accessToken}";

                await _stravaWebClient.SendPostAsync(new Uri(postUrl), gpxFilePath);
            }
            catch (Exception ex)
            {
                string title = $"StravaActivityService.UploadActivityAsync - gpxFilePath {gpxFilePath} - activityType {activityType.ToString()} - name {name} - commute {commute} - isPrivate {isPrivate}";
                _logService.LogException(title, ex);
            }
        }

        public async Task<List<ActivitySummary>> HydrateActivityData(string data)
        {
            var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();
            List<ActivitySummary> results;
            if (data != null)
            {
                results = Unmarshaller<List<ActivitySummary>>.Unmarshal(data).Select(activity =>
                {
                    StravaService.SetMetricUnits(activity, defaultDistanceUnitType);
                    if (!string.IsNullOrEmpty(activity.Map.SummaryPolyline))
                        activity.Map.GoogleImageApiUrl = $"http://maps.googleapis.com/maps/api/staticmap?sensor=false&maptype={"roadmap"}&size={480}x{220}&scale=2&path=weight:4|color:0xff0000ff|enc:{activity.Map.SummaryPolyline}&key={StravaIdentityConstants.GOOGLE_MAP_API}";

                    return activity;
                }).ToList();
            }
            else
            {
                results = new List<ActivitySummary>();
            }

            return results;
        }
    }
}
