using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Kliva.Helpers;

namespace Kliva.Services
{
    public class StravaAthleteService : IStravaAthleteService
    {
        private readonly ISettingsService _settingsService;

        //TODO: Glenn - When to Invalidate cache?
        private readonly ConcurrentDictionary<string, Task<AthleteSummary>> _cachedAthleteTasks = new ConcurrentDictionary<string, Task<AthleteSummary>>();

        public Athlete Athlete { get; set; }

        public StravaAthleteService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        private async Task<AthleteSummary> GetAthleteFromServiceAsync(string athleteId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();

                string getUrl = string.Format("{0}/{1}?access_token={2}", Endpoints.Athletes, athleteId, accessToken);
                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                return Unmarshaller<AthleteSummary>.Unmarshal(json);
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }

        /// <summary>
        /// Asynchronously receives the currently authenticated athlete.
        /// </summary>
        /// <returns>The currently authenticated athlete.</returns>
        public async Task<Athlete> GetAthleteAsync()
        {
            if (Athlete != null)
                return Athlete;

            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();

                string getUrl = $"{Endpoints.Athlete}?access_token={accessToken}";
                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                return Athlete = Unmarshaller<Athlete>.Unmarshal(json);
            }
            catch (Exception)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }

        /// <summary>
        /// Asynchronously receives the currently authenticated athlete.
        /// </summary>
        /// <param name="athleteId">The Strava Id of the athlete.</param>
        /// <returns>The AthleteSummary object of the athlete.</returns>
        public Task<AthleteSummary> GetAthleteAsync(string athleteId)
        {
            return _cachedAthleteTasks.GetOrAdd(athleteId, GetAthleteFromServiceAsync);
        }        
    }
}
