using GoogleAnalytics.Core;
using Kliva.Services.Interfaces;

namespace Kliva.Services
{
    public class GoogleAnalyticsService : IGoogleAnalyticsService
    {
        public Tracker Tracker { get; set; }
    }
}
