using GoogleAnalytics.Core;

namespace Kliva.Services.Interfaces
{
    public interface IGoogleAnalyticsService
    {
        Tracker Tracker { get; set; }
    }
}