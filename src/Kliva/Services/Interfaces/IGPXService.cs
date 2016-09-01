using System.Threading.Tasks;

namespace Kliva.Services.Interfaces
{
    public interface IGPXService
    {
        Task InitGPXDocument();
        Task EndGPXDocument();
    }
}
