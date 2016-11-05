using System;
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

        public void Log(string title, Exception exception)
        {
            Debug.WriteLine($"{title} -- {exception.Message}");
            Debug.WriteLine(exception.StackTrace);
        }
    }
}
