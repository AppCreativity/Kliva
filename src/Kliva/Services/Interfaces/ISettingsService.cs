using Kliva.Models;
using System.Threading.Tasks;

namespace Kliva.Services.Interfaces
{
    public interface ISettingsService : IApplicationInfoService
    {
        Task<string> GetStoredStravaAccessToken();
        Task SetStravaAccessTokenAsync(string stravaAccessToken);
        Task RemoveStravaAccessToken();

        Task<DistanceUnitType> GetStoredDistanceUnitTypeAsync();
        Task SetDistanceUnitType(DistanceUnitType distanceUnitType);

        //Task<AppVersion> GetStoredAppVersion();
    }
}
