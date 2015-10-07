using Kliva.Models;
using System.Threading.Tasks;

namespace Kliva.Services.Interfaces
{
    public interface IStravaAthleteService
    {
        Task<AthleteSummary> GetAthleteAsync(string athleteId);
    }
}
