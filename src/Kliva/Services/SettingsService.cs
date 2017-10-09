using Cimbalino.Toolkit.Services;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Kliva.Services
{
    public class SettingsService : ApplicationInfoService, ISettingsService
    {
        private Settings _settings;

        public SettingsService(IStorageService storageService) : base(storageService)
        {
        }

        public async Task SetStravaAccessTokenAsync(string stravaAccessToken)
        {
            await LoadSettingsAsync(createIfNotExisting: true);

            _settings.StravaAccessToken = stravaAccessToken;

            await SaveSettingsToStorageAsync();
        }

        public async Task<string> GetStoredStravaAccessTokenAsync()
        {
            await LoadSettingsAsync();
            return _settings?.StravaAccessToken;
        }

        public Task RemoveStravaAccessTokenAsync()
        {
            return SetStravaAccessTokenAsync(string.Empty);
        }

        public async Task<DistanceUnitType> GetStoredDistanceUnitTypeAsync()
        {
            await LoadSettingsAsync();
            return _settings?.DistanceUnitType ?? DistanceUnitType.Kilometres;
        }

        public async Task SetDistanceUnitTypeAsync(DistanceUnitType distanceUnitType)
        {
            await LoadSettingsAsync(createIfNotExisting: true);

            _settings.DistanceUnitType = distanceUnitType;

            await SaveSettingsToStorageAsync();
        }

        public async Task<ActivityRecording> GetStoredActivityRecordingTypeAsync()
        {
            await LoadSettingsAsync();
            return _settings?.ActivityRecordingType ?? ActivityRecording.Cycling;
        }

        public async Task SetActivityRecordingTypeAsync(ActivityRecording activityRecordingType)
        {
            await LoadSettingsAsync(createIfNotExisting: true);

            _settings.ActivityRecordingType = activityRecordingType;

            await SaveSettingsToStorageAsync();
        }

        public async Task<ActivityFeedFilter> GetStoredActivityFeedFilterAsync()
        {
            await LoadSettingsAsync();
            return _settings?.ActivityFeedFilter ?? ActivityFeedFilter.All;
        }

        public async Task SetActivityFeedFilterAsync(ActivityFeedFilter filter)
        {
            await LoadSettingsAsync(createIfNotExisting: true);

            _settings.ActivityFeedFilter = filter;

            await SaveSettingsToStorageAsync();
        }

        public async Task<ActivitySort> GetStoredActivitySortAsync()
        {
            await LoadSettingsAsync();
            return _settings?.ActivitySort ?? ActivitySort.StartTime;
        }

        public async Task SetActivitySortAsync(ActivitySort sort)
        {
            await LoadSettingsAsync(createIfNotExisting: true);

            _settings.ActivitySort = sort;

            await SaveSettingsToStorageAsync();
        }

        public async Task<AppVersion> GetStoredAppVersionAsync()
        {
            await LoadSettingsAsync();
            return _settings?.AppVersion ?? new AppVersion(new PackageVersion() { Major = 0, Minor = 0, Revision = 0, Build = 0 });
        }

        public async Task SetAppVersionAsync(AppVersion appVersion)
        {
            await LoadSettingsAsync(createIfNotExisting: true);

            _settings.AppVersion = appVersion;

            await SaveSettingsToStorageAsync();
        }

        private async Task LoadSettingsAsync(bool createIfNotExisting = false)
        {
            if (_settings == null)
            {
                bool settingsExists = await DoesSettingsServiceExistAsync();
                if (settingsExists)
                {
                    string settingsAsString = await StorageService.Local.ReadAllTextAsync(Constants.SETTINGSSTORE);
                    _settings = JsonConvert.DeserializeObject<Settings>(settingsAsString);
                }
                else if (createIfNotExisting)
                {
                    _settings = new Settings();
                }
            }
        }

        private Task SaveSettingsToStorageAsync()
        {
            string serializedSettings = JsonConvert.SerializeObject(_settings);
            return StorageService.Local.WriteAllTextAsync(Constants.SETTINGSSTORE, serializedSettings);
        }

        private Task<bool> DoesSettingsServiceExistAsync()
        {
            return StorageService.Local.FileExistsAsync(Constants.SETTINGSSTORE);
        }
    }
}