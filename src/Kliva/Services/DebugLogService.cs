using System.Diagnostics;
using Kliva.Services.Interfaces;

namespace Kliva.Services
{
    public class DebugLogService : ILogService
    {
        public void Log(string title, string body)
        {
            Debug.WriteLine($"{title} -- {body}");
        }
    }
}
