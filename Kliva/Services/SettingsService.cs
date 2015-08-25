using Cimbalino.Toolkit.Services;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Windows.ApplicationModel;

namespace Kliva.Services
{
    public class SettingsService : ApplicationInfoService, ISettingsService
    {
        private Settings _settings;

        private async Task<bool> SettingsServiceExists()
        {
            return await ServiceLocator.Current.GetInstance<IStorageService>().Local.FileExistsAsync(Constants.SETTINGSSTORE);
        }

        public async Task SetStravaAccessToken(string stravaAccessToken)
        {
            if (_settings == null)
            {
                bool settingsExists = await this.SettingsServiceExists();
                if (settingsExists)
                {
                    string settingsAsString = await ServiceLocator.Current.GetInstance<IStorageService>().Local.ReadAllTextAsync(Constants.SETTINGSSTORE);
                    _settings = JsonConvert.DeserializeObject<Settings>(settingsAsString);
                }
                else
                    _settings = new Settings();
            }

            _settings.StravaAccessToken = stravaAccessToken;

            await ServiceLocator.Current.GetInstance<IStorageService>().Local.WriteAllTextAsync(Constants.SETTINGSSTORE, JsonConvert.SerializeObject(_settings));
        }

        public async Task<string> GetStoredStravaAccessToken()
        {
            if (_settings != null)
                return _settings.StravaAccessToken;

            bool settingsExists = await this.SettingsServiceExists();
            if (settingsExists)
            {
                string settingsAsString = await ServiceLocator.Current.GetInstance<IStorageService>().Local.ReadAllTextAsync(Constants.SETTINGSSTORE);
                _settings = JsonConvert.DeserializeObject<Settings>(settingsAsString);
                return _settings.StravaAccessToken;
            }

            return null;
        }

        public async Task RemoveStravaAccessToken()
        {
            if (_settings != null)
                _settings.StravaAccessToken = string.Empty;

            await this.SetStravaAccessToken(string.Empty);
        }

        //public async Task<AppVersion> GetStoredAppVersion()
        //{
        //    if (_settings != null)
        //        return _settings.AppVersion;

        //    bool settingsExists = await this.SettingsServiceExists();
        //    if (settingsExists)
        //    {
        //        string settingsAsString = await ServiceLocator.Current.GetInstance<IStorageService>().Local.ReadAllTextAsync(Constants.SETTINGSSTORE);
        //        _settings = JsonConvert.DeserializeObject<Settings>(settingsAsString);
        //        return _settings.AppVersion;
        //    }

        //    return null;
        //}
    }
}
