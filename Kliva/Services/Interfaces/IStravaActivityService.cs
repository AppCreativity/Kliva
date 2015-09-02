using Kliva.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kliva.Services.Interfaces
{
    public interface IStravaActivityService
    {
        Task<List<ActivitySummary>> GetActivitiesAsync(int page, int perPage);
        Task<List<ActivitySummary>> GetFollowersActivitiesAsync(int page, int perPage);
    }
}
