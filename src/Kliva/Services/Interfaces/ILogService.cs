using System;

namespace Kliva.Services.Interfaces
{
    public interface ILogService
    {
        void Log(string category, string action, string label);
        void LogException(string title, Exception exception);
    }
}