using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Kliva.Helpers;
using Kliva.Services.Performance;

namespace Kliva.Services
{
    public class StravaAthleteService : IStravaAthleteService
    {
        private readonly ISettingsService _settingsService;
        private readonly StravaWebClient _stravaWebClient;

        private readonly ETWLogging _perflog;

        //TODO: Glenn - When to Invalidate cache?
        //
        private readonly ConcurrentDictionary<string, object> _cachedAthleteTasks = new ConcurrentDictionary<string, object>();

        public Athlete Athlete { get; set; }

        public StravaAthleteService(ISettingsService settingsService, StravaWebClient stravaWebClient)
        {
            _settingsService = settingsService;
            _stravaWebClient = stravaWebClient;

            _perflog = ETWLogging.Log;
        }

        private async Task<AthleteSummary> GetAthleteFromServiceAsync(string athleteId)
        {
            try
            {
                _perflog.GetAthleteFromServiceAsync(false, athleteId);
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();

                string getUrl = string.Format("{0}/{1}?access_token={2}", Endpoints.Athletes, athleteId, accessToken);
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                var result = Unmarshaller<AthleteSummary>.Unmarshal(json);

                if (result != null)
                    ConsolidateWithCache(result);

                _perflog.GetAthleteFromServiceAsync(true, athleteId);
                return result;
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
            _perflog.GetAthleteAsync(false);
            if (Athlete == null)
            {
                try
                {
                    string json = await LocalCacheService.ReadCacheData("Athlete");
                    if (json == null || json.Length < 10)
                    {
                        var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                        string getUrl = $"{Endpoints.Athlete}?access_token={accessToken}";
                        json = await _stravaWebClient.GetAsync(new Uri(getUrl));
                        LocalCacheService.PersistCacheData(json, "Athlete");
                    }
                    return Athlete = Unmarshaller<Athlete>.Unmarshal(json);
                }
                catch (Exception)
                {
                    //TODO: Glenn - Use logger to log errors ( Google )
                }
            }
            _perflog.GetAthleteAsync(true);
            return Athlete;
        }

        /// <summary>
        /// Asynchronously receives the currently authenticated athlete.
        /// </summary>
        /// <param name="athleteId">The Strava Id of the athlete.</param>
        /// <returns>The AthleteSummary object of the athlete.</returns>
        public Task<AthleteSummary> GetAthleteAsync(string athleteId)
        {
            object result;
            if (_cachedAthleteTasks.TryGetValue(athleteId, out result))
            {
                AthleteSummary summary = result as AthleteSummary;
                if (summary != null)
                {
                    return Task.FromResult(summary);
                }
                else
                    return (Task<AthleteSummary>)result;
            }
            else
            {
                var task = GetAthleteFromServiceAsync(athleteId);
                _cachedAthleteTasks[athleteId] = task;
                return task;
            }
        }

        public async Task<IEnumerable<AthleteSummary>> GetFollowersAsync(string athleteId, bool authenticatedUser = true)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                string getUrl;

                if(authenticatedUser)
                    getUrl = $"{Endpoints.OwnFollowers}?access_token={accessToken}";
                else
                    getUrl = $"{string.Format(Endpoints.OtherFollowers, athleteId)}?access_token={accessToken}";

                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

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
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                string getUrl;

                if (authenticatedUser)
                    getUrl = $"{Endpoints.OwnFriends}?access_token={accessToken}";
                else
                    getUrl = $"{string.Format(Endpoints.OtherFriends, athleteId)}?access_token={accessToken}";

                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

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
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                string getUrl = $"{string.Format(Endpoints.MutualFriends, athleteId)}?access_token={accessToken}";

                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

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
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();

                string getUrl = $"{string.Format(Endpoints.Koms, athleteId)}?access_token={accessToken}";

                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                return Unmarshaller<IEnumerable<SegmentEffort>>.Unmarshal(json).Select(segment =>
                {
                    StravaService.SetMetricUnits(segment, defaultDistanceUnitType);
                    return segment;
                }).ToList();
            }
            catch (Exception)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }

        public async Task<Stats> GetStatsAsync(string athleteId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessTokenAsync();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();

                string getUrl = $"{string.Format(Endpoints.Stats, athleteId)}?access_token={accessToken}";

                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                Stats stats = Unmarshaller<Stats>.Unmarshal(json);
                StravaService.SetMetricUnits(stats, defaultDistanceUnitType);

                return stats;
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }

        public AthleteSummary ConsolidateWithCache(AthleteMeta athlete)
        {
            object entry;
            if (_cachedAthleteTasks.TryGetValue(athlete.Id.ToString(), out entry))
            {
                //The cache has an entry, so lets see which is newer and has more data
                AthleteSummary c_summary = entry as AthleteSummary;
                AthleteSummary n_summary = athlete as AthleteSummary;

                if (c_summary == null || (n_summary != null && n_summary.UpdatedAt > c_summary.UpdatedAt && n_summary.ResourceState >= c_summary.ResourceState))
                {
                    _cachedAthleteTasks[athlete.Id.ToString()] = n_summary;
                    return n_summary;
                }
                return c_summary;
            }
            else
            {
                if (athlete.ResourceState > 1) // meta only
                {
                    _cachedAthleteTasks[athlete.Id.ToString()] = athlete;
                    return (AthleteSummary)athlete;
                }
                return null;
            }
        }

    }
}
