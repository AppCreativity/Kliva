using Kliva.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kliva.Services.Interfaces
{
    public interface IStravaActivityService
    {
        Task<Activity> GetActivityAsync(string id, bool includeEfforts);
        Task<IList<ActivitySummary>> GetActivitiesAsync(int page, int perPage);
        Task<IList<ActivitySummary>> GetFollowersActivitiesAsync(int page, int perPage);

        Task<List<Athlete>> GetKudosAsync(string activityId);
        Task GiveKudosAsync(string activityId);

        Task<List<Comment>> GetCommentsAsync(string activityId);

        Task<List<Photo>> GetPhotosAsync(string activityId);
    }
}
