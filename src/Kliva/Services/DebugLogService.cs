using System;
using System.Diagnostics;
using Kliva.Services.Interfaces;

namespace Kliva.Services
{
    public class DebugLogService : ILogService
    {
        public void Log(string category, string action, string label)
        {
            Debug.WriteLine($"Category: {category} - Action: {action} - Label: {label}");
        }

        public void LogException(string title, Exception exception)
        {
            Debug.WriteLine($"{title} -- {exception.Message}");
            Debug.WriteLine(exception.StackTrace);
        }
    }
}
