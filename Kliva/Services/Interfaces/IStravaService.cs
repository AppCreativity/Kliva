using System;
using System.Threading.Tasks;

namespace Kliva.Services.Interfaces
{
    public interface IStravaService
    {
        event EventHandler<StravaServiceEventArgs> StatusEvent;

        Task GetAuthorizationCode();
    }
}
