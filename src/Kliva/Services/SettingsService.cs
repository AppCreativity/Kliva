using Cimbalino.Toolkit.Services;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kliva.Services
{
    public class SettingsService : ApplicationInfoService, ISettingsService
    {
        private readonly IStorageService _storageService;
        private Settings _settings;

        public SettingsService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task SetStravaAccessTokenAsync(string stravaAccessToken)
        {
            await LoadSettings(createIfNotExisting: true);

            _settings.StravaAccessToken = stravaAccessToken;

            await SaveSettingsToStorage();
        }

        public async Task<string> GetStoredStravaAccessToken()
        {
            await LoadSettings();
            return _settings?.StravaAccessToken;
        }

        public Task RemoveStravaAccessToken()
        {
            return SetStravaAccessTokenAsync(string.Empty);
        }

        public async Task<DistanceUnitType> GetStoredDistanceUnitType()
        {
            await LoadSettings();
            return _settings?.DistanceUnitType ?? DistanceUnitType.Kilometres;
        }

        public async Task SetDistanceUnitType(DistanceUnitType distanceUnitType)
        {
            await LoadSettings(createIfNotExisting: true);

            _settings.DistanceUnitType = distanceUnitType;

            await SaveSettingsToStorage();
        }

        private async Task LoadSettings(bool createIfNotExisting = false)
        {
            if (_settings == null)
            {
                bool settingsExists = await DoesSettingsServiceExist();
                if (settingsExists)
                {
                    string settingsAsString = await _storageService.Local.ReadAllTextAsync(Constants.SETTINGSSTORE);
                    _settings = JsonConvert.DeserializeObject<Settings>(settingsAsString);
                }
                else if (createIfNotExisting)
                {
                    _settings = new Settings();
                }
            }
        }

        private Task SaveSettingsToStorage()
        {
            string serializedSettings = JsonConvert.SerializeObject(_settings);
            return _storageService.Local.WriteAllTextAsync(Constants.SETTINGSSTORE, serializedSettings);
        }

        private Task<bool> DoesSettingsServiceExist()
        {
            return _storageService.Local.FileExistsAsync(Constants.SETTINGSSTORE);
        }
    }
}