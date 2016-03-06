using System;
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

        public StravaSegmentService(ISettingsService settingsService, StravaWebClient stravaWebClient)
        {
            _settingsService = settingsService;
            _stravaWebClient = stravaWebClient;
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
    }
}
