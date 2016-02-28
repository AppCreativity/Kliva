using System.Collections.Generic;
using System.Threading.Tasks;
using Kliva.Models;

namespace Kliva.Services.Interfaces
{
    public interface IStravaClubService
    {
        Task<List<ClubSummary>> GetClubsAsync();
        Task<Club> GetClubAsync(string id);
        Task<List<AthleteSummary>> GetClubMembersAsync(string clubId);
    }
}
