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

        Task<List<ActivitySummary>> GetActivitiesWithAthletesAsync(int page, int perPage);
    }
}
