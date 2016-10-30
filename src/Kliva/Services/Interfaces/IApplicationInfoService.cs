using System.Collections.Generic;
using System.Threading.Tasks;
using Kliva.Models;

namespace Kliva.Services.Interfaces
{
    public interface IApplicationInfoService
    {
        AppVersion AppVersion { get; }
        Task<List<ApplicationInfo>> GetAppInfoAsync();
    }
}