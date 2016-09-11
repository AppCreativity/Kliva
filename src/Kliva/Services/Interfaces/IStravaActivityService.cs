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
        Task<IList<ActivitySummary>> GetRelatedActivitiesAsync(string activityId);

        Task<List<AthleteSummary>> GetKudosAsync(string activityId);
        Task GiveKudosAsync(string activityId);

        Task<List<Comment>> GetCommentsAsync(string activityId);
        Task PostComment(string activityId, string text);

        Task PutUpdate(string activityId, string name, bool commute, bool isPrivate, string gearID);

        Task<List<Photo>> GetPhotosAsync(string activityId);

        Task<string> GetFriendActivityDataAsync(int page, int pageSize);
        Task<string> GetMyActivityDataAsync(int page, int pageSize);

        Task UploadActivityAsync(string gpxFilePath, ActivityType activityType, string name, bool commute, bool isPrivate);

        Task<List<ActivitySummary>> HydrateActivityData(string data);
    }
}
