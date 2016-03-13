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

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(async () => await ViewLoaded()));

        public SettingsViewModel(INavigationService navigationService, ISettingsService settingsService) : base(navigationService)
        {
            _settingsService = settingsService;

            this.PropertyChanged += OnSettingsViewModelPropertyChanged;

            //TODO: Glenn - Translate enums!
            MeasurementUnits.Add(DistanceUnitType.Kilometres.ToString());
            MeasurementUnits.Add(DistanceUnitType.Miles.ToString());
        }

        private async void OnSettingsViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SelectedMeasurementUnit", StringComparison.OrdinalIgnoreCase))
                await _settingsService.SetDistanceUnitTypeAsync(Enum<DistanceUnitType>.Parse(SelectedMeasurementUnit));
        }

        private async Task ViewLoaded()
        {
            DistanceUnitType distanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();
            SelectedMeasurementUnit = distanceUnitType.ToString();
        }
    }
}
