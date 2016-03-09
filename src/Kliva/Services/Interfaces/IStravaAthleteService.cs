using System.Collections.Generic;
using Kliva.Models;
using System.Threading.Tasks;

namespace Kliva.Services.Interfaces
{
    public interface IStravaAthleteService
    {
        Athlete Athlete { get; }
        Task<Athlete> GetAthleteAsync();
        Task<AthleteSummary> GetAthleteAsync(string athleteId);

        Task<IEnumerable<AthleteSummary>> GetFollowersAsync(string athleteId, bool authenticatedUser = true);
        Task<IEnumerable<AthleteSummary>> GetFriendsAsync(string athleteId, bool authenticatedUser = true);
        Task<IEnumerable<AthleteSummary>> GetMutualFriendsAsync(string athleteId);
        Task<IEnumerable<SegmentEffort>> GetKomsAsync(string athleteId);

        AthleteSummary ConsolidateWithCache(AthleteMeta athlete);
    }
}
