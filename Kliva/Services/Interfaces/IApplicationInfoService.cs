using Kliva.Models;

namespace Kliva.Services.Interfaces
{
    public interface IApplicationInfoService
    {
        AppVersion AppVersion { get; }
    }
}