using Kliva.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kliva.Services.Interfaces
{
    public interface IStravaService
    {
        IStravaActivityService StravaActivityService { get; }

        event EventHandler<StravaServiceEventArgs> StatusEvent;

        Task GetAuthorizationCode();

        Task<Activity> GetActivityAsync(string id, bool includeEfforts);
        Task<IEnumerable<ActivitySummary>> GetActivitiesWithAthletesAsync(int page, int perPage);
    }
}
