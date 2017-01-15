using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Models;
using Kliva.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Globalization.NumberFormatting;
using Windows.Storage;
using Windows.Storage.FileProperties;
using GalaSoft.MvvmLight.Threading;
using Kliva.Extensions;
using Kliva.Services;
using GalaSoft.MvvmLight.Messaging;

namespace Kliva.ViewModels
{
    public class SettingsViewModel : KlivaBaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IOService _ioService;

        private bool _loading;
        private DecimalFormatter _decimalFormat = new DecimalFormatter();

        public string AppVersion => _settingsService.AppVersion.ToString();

        private ObservableCollection<string> _measurementUnits = new ObservableCollection<string>();
        public ObservableCollection<string> MeasurementUnits
        {
            get { return _measurementUnits; }
            set { Set(() => MeasurementUnits, ref _measurementUnits, value); }
        }

        private string _selectedMeasurementUnit;
        public string SelectedMeasurementUnit
        {
            get { return _selectedMeasurementUnit; }
            set { Set(() => SelectedMeasurementUnit, ref _selectedMeasurementUnit, value); }
        }

        private ObservableCollection<string> _sortTypes = new ObservableCollection<string>();
        public ObservableCollection<string> SortTypes
        {
            get { return _sortTypes; }
            set { Set(() => SortTypes, ref _sortTypes, value); }
        }

        private string _selectedSortType;
        public string SelectedSortType
        {
            get { return _selectedSortType; }
            set { Set(() => SelectedSortType, ref _selectedSortType, value); }
        }

        private string _mapSizes;
        public string MapSizes
        {
            get { return _mapSizes; }
            set { Set(() => MapSizes, ref _mapSizes, value); }
        }

        private ApplicationInfo _currentVersion;
        public ApplicationInfo CurrentVersion
        {
            get { return _currentVersion; }
            set { Set(() => CurrentVersion, ref _currentVersion, value); }
        }

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(async () => await ViewLoaded()));

        private RelayCommand _clearMapsCommand;
        public RelayCommand ClearMapsCommand => _clearMapsCommand ?? (_clearMapsCommand = new RelayCommand(async () => await ClearMaps()));

        public SettingsViewModel(INavigationService navigationService, ISettingsService settingsService, IOService ioService, IMessenger messenger) : base(navigationService, messenger)
        {
            _settingsService = settingsService;
            _ioService = ioService;

            PropertyChanged += OnSettingsViewModelPropertyChanged;

            IncrementNumberRounder rounder = new IncrementNumberRounder();
            rounder.Increment = 0.01;
            rounder.RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp;
            _decimalFormat.NumberRounder = rounder;            

            //TODO: Glenn - Translate enums!
            MeasurementUnits.Add(DistanceUnitType.Kilometres.ToString());
            MeasurementUnits.Add(DistanceUnitType.Miles.ToString());

            SortTypes.Add(ActivitySort.StartTime.ToString());
            SortTypes.Add(ActivitySort.EndTime.ToString());
        }

        private async void OnSettingsViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_loading)
                return;

            if (e.PropertyName.Equals("SelectedMeasurementUnit", StringComparison.OrdinalIgnoreCase))
            {
                var newValue = Enum<DistanceUnitType>.Parse(SelectedMeasurementUnit);
                MessengerInstance.Send(new Messages.MeasureUnitChangedMessage(newValue));
                await _settingsService.SetDistanceUnitTypeAsync(newValue);
            }

            else if (e.PropertyName.Equals("SelectedSortType", StringComparison.OrdinalIgnoreCase))
                await _settingsService.SetActivitySortAsync(Enum<ActivitySort>.Parse(SelectedSortType));
        }

        private async Task ViewLoaded()
        {
            _loading = true;

            DistanceUnitType distanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();
            SelectedMeasurementUnit = distanceUnitType.ToString();

            ActivitySort activitySort = await _settingsService.GetStoredActivitySortAsync();
            SelectedSortType = activitySort.ToString();

            var appInfoList = await _settingsService.GetAppInfoAsync();
            CurrentVersion = appInfoList.FirstOrDefault();
            //TODO: Glenn - refactor initialization in settings service? While converting json to class?
            if (CurrentVersion.Features != null)
            {
                InfoOverviewItem features = new InfoOverviewItem()
                {
                    Header = "Features",
                    Items = new ObservableCollection<string>(CurrentVersion.Features)
                };
                CurrentVersion.OverviewItems.Add(features);
            }

            if (CurrentVersion.BugFixes != null)
            {
                InfoOverviewItem bugFixes = new InfoOverviewItem()
                {
                    Header = "Bug fixes",
                    Items = new ObservableCollection<string>(CurrentVersion.BugFixes)
                };
                CurrentVersion.OverviewItems.Add(bugFixes);
            }                      

            await Task.Run(GetMapSizes);

            _loading = false;
        }

        private async Task<IReadOnlyList<StorageFile>> GetMapFiles()
        {
            List<string> fileTypes = new List<string>() { ".map" };
            var mapFiles = await _ioService.GetFilesAsync(fileTypes);

            return mapFiles;
        }


        private async Task GetMapSizes()
        {
            var mapFiles = await GetMapFiles();

            long totalFileSize = 0;
            foreach (StorageFile mapFile in mapFiles)
            {
                BasicProperties basicProperties = await mapFile.GetBasicPropertiesAsync();
                totalFileSize += Convert.ToInt64(basicProperties.Size);
            }

            double mapSizeInMB = totalFileSize > 0 ? (totalFileSize / 1024.0) / 1024.0 : 0;

            DispatcherHelper.CheckBeginInvokeOnUI(() => MapSizes = $"Currently we have {mapFiles.Count} maps cached, with a total of {_decimalFormat.Format(mapSizeInMB)} MB");
        }

        private async Task ClearMaps()
        {
            IsBusy = true;

            var mapFiles = await GetMapFiles();            

            foreach (StorageFile mapFile in mapFiles)
            {
                try
                {
                    await mapFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
                catch (Exception exception)
                {
                    //TODO: Glenn - implement
                }
            }

            await GetMapSizes();

            IsBusy = false;
        }
    }
}
