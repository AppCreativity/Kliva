using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private readonly ConcurrentDictionary<string, Task<Athlete>> _cachedAthleteTasks = new ConcurrentDictionary<string, Task<Athlete>>();

        public Athlete Athlete { get; set; }

        public StravaAthleteService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<Athlete> GetAthleteFromServiceAsync(string athleteId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();

                string getUrl = string.Format("{0}/{1}?access_token={2}", Endpoints.Athletes, athleteId, accessToken);
                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                return Unmarshaller<Athlete>.Unmarshal(json);
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
        public Task<Athlete> GetAthleteAsync(string athleteId)
        {
            return _cachedAthleteTasks.GetOrAdd(athleteId, GetAthleteFromServiceAsync);
        }

        public async Task<IEnumerable<AthleteSummary>> GetFollowersAsync(string athleteId, bool authenticatedUser = true)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();
                string getUrl;

                if(authenticatedUser)
                    getUrl = $"{Endpoints.OwnFollowers}?access_token={accessToken}";
                else
                    getUrl = $"{string.Format(Endpoints.OtherFollowers, athleteId)}?access_token={accessToken}";

                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                return Unmarshaller<IEnumerable<AthleteSummary>>.Unmarshal(json);
            }
            catch (Exception)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }

        public async Task<IEnumerable<AthleteSummary>> GetFriendsAsync(string athleteId, bool authenticatedUser = true)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();
                string getUrl;

                if (authenticatedUser)
                    getUrl = $"{Endpoints.OwnFriends}?access_token={accessToken}";
                else
                    getUrl = $"{string.Format(Endpoints.OtherFriends, athleteId)}?access_token={accessToken}";

                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                return Unmarshaller<IEnumerable<AthleteSummary>>.Unmarshal(json);
            }
            catch (Exception)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;

        }

        public async Task<IEnumerable<AthleteSummary>> GetMutualFriendsAsync(string athleteId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();
                string getUrl = $"{string.Format(Endpoints.MutualFriends, athleteId)}?access_token={accessToken}";

                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                return Unmarshaller<IEnumerable<AthleteSummary>>.Unmarshal(json);
            }
            catch (Exception)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;

        }

        public async Task<IEnumerable<SegmentEffort>> GetKomsAsync(string athleteId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();
                string getUrl = $"{string.Format(Endpoints.Koms, athleteId)}?access_token={accessToken}";

                string json = await WebRequest.SendGetAsync(new Uri(getUrl));

                return Unmarshaller<IEnumerable<SegmentEffort>>.Unmarshal(json);
            }
            catch (Exception)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;

        }
    }
}
