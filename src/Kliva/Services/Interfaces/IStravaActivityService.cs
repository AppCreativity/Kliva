using Kliva.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kliva.Services.Interfaces
{
    public interface IStravaActivityService
    {
        Task<Activity> GetActivityAsync(string id, bool includeEfforts);
        Task<IEnumerable<ActivitySummary>> GetActivitiesAsync(int page, int perPage);
        Task<IEnumerable<ActivitySummary>> GetFollowersActivitiesAsync(int page, int perPage);

        Task<List<Photo>> GetPhotosAsync(string activityId);
    }
}
