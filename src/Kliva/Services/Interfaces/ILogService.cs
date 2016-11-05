using System;

namespace Kliva.Services.Interfaces
{
    public interface ILogService
    {
        void Log(string title, string body);
        void Log(string title, Exception exception);
    }
}