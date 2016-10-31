using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Windows.ApplicationModel;
using Cimbalino.Toolkit.Services;
using Newtonsoft.Json;

namespace Kliva.Services
{
    public class ApplicationInfoService : IApplicationInfoService
    {
        protected readonly IStorageService StorageService;

        private AppVersion _appVersion;
        public AppVersion AppVersion => _appVersion ?? (_appVersion = new AppVersion(Package.Current.Id.Version));

        public ApplicationInfoService(IStorageService storageService)
        {
            StorageService = storageService;
        }

        public async Task<List<ApplicationInfo>> GetAppInfoAsync()
        {
            try
            {
                string appInfo = await StorageService.Package.ReadAllTextAsync("AppInfo.json");
                List<ApplicationInfo> appInfoList = JsonConvert.DeserializeObject<List<ApplicationInfo>>(appInfo);

                return appInfoList.OrderByDescending(item => item.Version).ToList();
            }
            catch (Exception)
            {
            }

            return null;
        }
    }
}
