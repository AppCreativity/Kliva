using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Kliva.Helpers;

namespace Kliva.Services
{
    public class StravaActivityService : IStravaActivityService
    {
        private ISettingsService _settingsService;

        public StravaActivityService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// Gets all the activities asynchronously. Pagination is supported.
        /// </summary>
        /// <param name="page">The page of activities.</param>
        /// <param name="perPage">The amount of activities that are loaded per page.</param>
        /// <returns>A list of activities.</returns>
        public async Task<List<ActivitySummary>> GetActivitiesAsync(int page, int perPage)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();

                //TODO: Glenn - Optional parameters should be treated as such!
                //string getUrl = String.Format("{0}?page={1}&per_page={2}&access_token={3}", Endpoints.Activities, page, perPage, accessToken);
                string getUrl = String.Format("{0}?access_token={1}", Endpoints.Activities, accessToken);
                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                //TODO: Glenn - Google maps?
                return Unmarshaller<List<ActivitySummary>>.Unmarshal(json);
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
        public async Task<List<ActivitySummary>> GetFollowersActivitiesAsync(int page, int perPage)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();

                //TODO: Glenn - Optional parameters should be treated as such!
                //string getUrl = String.Format("{0}?page={1}&per_page={2}&access_token={3}", Endpoints.ActivitiesFollowers, page, perPage, accessToken);
                string getUrl = string.Format("{0}?&access_token={1}", Endpoints.ActivitiesFollowers, accessToken);
                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                return Unmarshaller<List<ActivitySummary>>.Unmarshal(json);
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }
    }
}
