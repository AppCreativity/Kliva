using Kliva.Helpers;
using Kliva.Models;
using Kliva.Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kliva.Services.Performance;

namespace Kliva.Services
{
    public class StravaActivityService : IStravaActivityService
    {
        private readonly ISettingsService _settingsService;
        private readonly StravaWebClient _stravaWebClient;

        private readonly ETWLogging _perflog;

        //TODO: Glenn - When to Invalidate cache?
        private readonly ConcurrentDictionary<string, Task<List<Photo>>> _cachedPhotosTasks = new ConcurrentDictionary<string, Task<List<Photo>>>();

        //TODO: Glenn - When to Invalidate cache?
        private readonly ConcurrentDictionary<string, Task<IList<ActivitySummary>>> _cachedRelatedActivitiesTasks = new ConcurrentDictionary<string, Task<IList<ActivitySummary>>>();

        public StravaActivityService(ISettingsService settingsService, StravaWebClient stravaWebClient)
        {
            _settingsService = settingsService;
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
                //TODO: Glenn - Use logger to log errors ( Google )
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
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
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

                var activity = Unmarshaller<Activity>.Unmarshal(json);
                StravaService.SetMetricUnits(activity, defaultDistanceUnitType);
                if (activity.SegmentEfforts != null)
                {
                    foreach (SegmentEffort segment in activity.SegmentEfforts)
                        StravaService.SetMetricUnits(segment, defaultDistanceUnitType);
                }

                _perflog.GetActivityAsync(true, id, includeEfforts);
                return activity;
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
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
                //TODO: Glenn - Use logger to log errors ( Google )
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
                //TODO: Glenn - Use logger to log errors ( Google )
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
            catch (Exception)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
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
            catch (Exception)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
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
            catch (Exception)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
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
                //TODO: Glenn - Use logger to log errors ( Google )
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
                //TODO: Glenn - Use logger to log errors ( Google )
            }
            return data;
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
