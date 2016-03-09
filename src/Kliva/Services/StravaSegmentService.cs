using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Kliva.Helpers;
using Kliva.Models;
using Kliva.Services.Interfaces;

namespace Kliva.Services
{
    public class StravaSegmentService : IStravaSegmentService
    {
        private readonly ISettingsService _settingsService;
        private readonly StravaWebClient _stravaWebClient;

        private readonly ConcurrentDictionary<string, Task<List<SegmentSummary>>> _cachedStarredSegmentsTasks = new ConcurrentDictionary<string, Task<List<SegmentSummary>>>();


        public StravaSegmentService(ISettingsService settingsService, StravaWebClient stravaWebClient)
        {
            _settingsService = settingsService;
            _stravaWebClient = stravaWebClient;
        }

        /// <summary>
        /// Gets all the starred segments of an Athlete.
        /// </summary>
        /// <returns>A list of segments that are starred by the athlete.</returns>
        private async Task<List<SegmentSummary>> GetStarredSegmentsFromServiceAsync(string athleteId)
        {
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();

                string getUrl = $"{Endpoints.Athletes}/{athleteId}/segments/starred?access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                var segments = Unmarshaller<List<SegmentSummary>>.Unmarshal(json);
                foreach (SegmentSummary segment in segments)
                    StravaService.SetMetricUnits(segment, defaultDistanceUnitType);

                return segments;
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }

        public async Task<List<SegmentSummary>> GetStarredSegmentsAsync()
        {
            //TODO: Glenn - Caching?
            try
            {
                var accessToken = await _settingsService.GetStoredStravaAccessToken();
                var defaultDistanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();

                string getUrl = $"{Endpoints.Starred}?access_token={accessToken}";
                string json = await _stravaWebClient.GetAsync(new Uri(getUrl));

                var segments = Unmarshaller<List<SegmentSummary>>.Unmarshal(json);
                foreach (SegmentSummary segment in segments)
                    StravaService.SetMetricUnits(segment, defaultDistanceUnitType);

                return segments;
            }
            catch (Exception ex)
            {
                //TODO: Glenn - Use logger to log errors ( Google )
            }

            return null;
        }

        public Task<List<SegmentSummary>> GetStarredSegmentsAsync(string athleteId)
        {
            return _cachedStarredSegmentsTasks.GetOrAdd(athleteId, GetStarredSegmentsFromServiceAsync);
        }
    }
}
