using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kliva.Models;
using Kliva.Services.Interfaces;

namespace Kliva.Services
{
    public class StravaActivityService : IStravaActivityService
    {
        /// <summary>
        /// Gets all the activities asynchronously. Pagination is supported.
        /// </summary>
        /// <param name="page">The page of activities.</param>
        /// <param name="perPage">The amount of activities that are loaded per page.</param>
        /// <returns>A list of activities.</returns>
        public async Task<List<ActivitySummary>> GetActivitiesAsync(int page, int perPage)
        {
            string getUrl = String.Format("{0}?page={1}&per_page={2}&access_token={3}", Endpoints.Activities, page, perPage, Authentication.AccessToken);
            string json = await Http.WebRequest.SendGetAsync(new Uri(getUrl));
        }
    }
}
