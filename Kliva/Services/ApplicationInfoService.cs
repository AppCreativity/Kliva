using System;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Windows.ApplicationModel;

namespace Kliva.Services
{
    public class ApplicationInfoService : IApplicationInfoService
    {
        private AppVersion _appVersion;
        public AppVersion AppVersion
        {
            get { return _appVersion ?? (_appVersion = new AppVersion(Package.Current.Id.Version)); }
        }
    }
}
