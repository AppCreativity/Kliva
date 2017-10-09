using Kliva.Models;
using System.Threading.Tasks;

namespace Kliva.Services.Interfaces
{
    public interface ISettingsService : IApplicationInfoService
    {
        Task<string> GetStoredStravaAccessTokenAsync();
        Task SetStravaAccessTokenAsync(string stravaAccessToken);
        Task RemoveStravaAccessTokenAsync();

        Task<DistanceUnitType> GetStoredDistanceUnitTypeAsync();
        Task SetDistanceUnitTypeAsync(DistanceUnitType distanceUnitType);

        Task<ActivityFeedFilter> GetStoredActivityFeedFilterAsync();
        Task SetActivityFeedFilterAsync(ActivityFeedFilter filter);

        Task<ActivitySort> GetStoredActivitySortAsync();
        Task SetActivitySortAsync(ActivitySort sort);

        Task<ActivityRecording> GetStoredActivityRecordingTypeAsync();
        Task SetActivityRecordingTypeAsync(ActivityRecording activityRecordingType);

        Task<AppVersion> GetStoredAppVersionAsync();
        Task SetAppVersionAsync(AppVersion appVersionB);
    }
}
