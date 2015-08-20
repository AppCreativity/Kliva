using System.Threading.Tasks;

namespace Kliva.Services.Interfaces
{
    public interface IStravaService
    {
        Task GetAuthorizationCode();
    }
}
