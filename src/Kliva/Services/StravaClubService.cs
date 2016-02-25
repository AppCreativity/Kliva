using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kliva.Helpers;
using Kliva.Models;
using Kliva.Services.Interfaces;

namespace Kliva.Services
{
    public class StravaClubService : IStravaClubService
    {
        private readonly ISettingsService _settingsService;

        public StravaClubService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <summary>
        /// Gets a list of clubs in which the currently authenticated athlete is a member of.
        /// </summary>
        /// <returns>The list of clubs in which the currently authenticated user is a member of.</returns>
        public async Task<List<ClubSummary>> GetClubsAsync()
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();
                string getUrl = $"{Endpoints.Clubs}?access_token={accessToken}";
                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                return Unmarshaller<List<ClubSummary>>.Unmarshal(json);
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }
    }
}
