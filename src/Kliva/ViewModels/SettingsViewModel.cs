using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Helpers;
using Kliva.Models;
using Kliva.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Kliva.ViewModels
{
    public class SettingsViewModel : KlivaBaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private bool _loading;

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

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(async () => await ViewLoaded()));

        public SettingsViewModel(INavigationService navigationService, ISettingsService settingsService) : base(navigationService)
        {
            _settingsService = settingsService;

            PropertyChanged += OnSettingsViewModelPropertyChanged;

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
                await _settingsService.SetDistanceUnitTypeAsync(Enum<DistanceUnitType>.Parse(SelectedMeasurementUnit));

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

            _loading = false;
        }
    }
}
