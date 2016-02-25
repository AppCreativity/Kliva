using System.Collections.Generic;
using System.Threading.Tasks;
using Kliva.Models;

namespace Kliva.Services.Interfaces
{
    public interface IStravaClubService
    {
        Task<List<ClubSummary>> GetClubsAsync();
    }
}
