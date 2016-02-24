using Kliva.Helpers;
using Kliva.Models;
using Kliva.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kliva.Services
{
    public class StravaActivityService : IStravaActivityService
    {
        private ISettingsService _settingsService;

        public StravaActivityService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        private void SetMetricUnits(ActivitySummary activity, DistanceUnitType distanceUnitType)
        {            
            activity.DistanceUnit = distanceUnitType;
            activity.SpeedUnit = activity.DistanceUnit == DistanceUnitType.Kilometres ? SpeedUnit.KilometresPerHour : SpeedUnit.MilesPerHour;
            activity.ElevationUnit = activity.DistanceUnit == DistanceUnitType.Kilometres ? DistanceUnitType.Metres : DistanceUnitType.Feet;
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
                var accessToken = await _settingsService.GetStoredStravaAccessToken();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitType();

                string getUrl = $"{Endpoints.Activity}/{id}?include_all_efforts={includeEfforts}&access_token={accessToken}";
                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                var activity = Unmarshaller<Activity>.Unmarshal(json);
                SetMetricUnits(activity, defaultDistanceUnitType);

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
        public async Task<IEnumerable<ActivitySummary>> GetActivitiesAsync(int page, int perPage)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitType();

                //TODO: Glenn - Optional parameters should be treated as such!
                //string getUrl = String.Format("{0}?page={1}&per_page={2}&access_token={3}", Endpoints.Activities, page, perPage, accessToken);
                string getUrl = $"{Endpoints.Activities}?access_token={accessToken}";
                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                //TODO: Glenn - Google maps?
                return Unmarshaller<List<ActivitySummary>>.Unmarshal(json).Select(activity =>
                {
                    SetMetricUnits(activity, defaultDistanceUnitType);
                    return activity;
                });
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
        public async Task<IEnumerable<ActivitySummary>> GetFollowersActivitiesAsync(int page, int perPage)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitType();

                //TODO: Glenn - Optional parameters should be treated as such!
                string getUrl = $"{Endpoints.ActivitiesFollowers}?page={page}&per_page={perPage}&access_token={accessToken}";
                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                return Unmarshaller<List<ActivitySummary>>.Unmarshal(json).Select(activity =>
                {
                    SetMetricUnits(activity, defaultDistanceUnitType);
                    return activity;
                });
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }

        /// <summary>
        /// Gets a list of athletes that kudoed the specified activity asynchronously.
        /// </summary>
        /// <param name="activityId">The Strava Id of the activity.</param>
        /// <returns>A list of athletes that kudoed the specified activity.</returns>
        public async Task<List<Athlete>> GetKudosAsync(string activityId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();

                string getUrl = $"{Endpoints.Activity}/{activityId}/kudos?access_token={accessToken}";
                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                return Unmarshaller<List<Athlete>>.Unmarshal(json);
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
                var accessToken = await _settingsService.GetStoredStravaAccessToken();

                string postUrl = $"{Endpoints.Activity}/{activityId}/kudos?access_token={accessToken}";
                await WebRequest.SendPostAsync(new Uri(postUrl));
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
                var accessToken = await _settingsService.GetStoredStravaAccessToken();

                string getUrl = $"{Endpoints.Activity}/{activityId}/comments?access_token={accessToken}";
                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

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
        public async Task<List<Photo>> GetPhotosAsync(string activityId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();

                string getUrl = $"{Endpoints.Activity}/{activityId}/photos?access_token={accessToken}";
                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                return Unmarshaller<List<Photo>>.Unmarshal(json);
            }
            catch(Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }
    }
}
