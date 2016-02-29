using Kliva.Models;
using System.Threading.Tasks;

namespace Kliva.Services.Interfaces
{
    public interface IStravaAthleteService
    {
        Athlete Athlete { get; }
        Task<Athlete> GetAthleteAsync();
        Task<AthleteSummary> GetAthleteAsync(string athleteId);
    }
}
