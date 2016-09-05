using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Devices.Geolocation;
using Windows.System.Threading;
using Windows.UI.Core;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Kliva.Extensions;
using Kliva.Models;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class RecordViewModel : KlivaBaseViewModel
    {
        private bool _loading = false;

        private ExtendedExecutionSession _extendedExecutionSession;
        private Timer _periodicTimer = null;
        private bool _canRecord = false;
        private readonly ILocationService _locationService;
        private readonly IGPXService _gpxService;

        private string _activityText;
        public string ActivityText
        {
            get { return _activityText; }
            set { Set(() => ActivityText, ref _activityText, value); }
        }

        private Geopoint _currentLocation;
        public Geopoint CurrentLocation
        {
            get { return _currentLocation; }
            set { Set(() => CurrentLocation, ref _currentLocation, value); }
        }

        private bool _isRecording = false;
        public bool IsRecording
        {
            get { return _isRecording; }
            set { Set(() => IsRecording, ref _isRecording, value); }
        }

        private bool _isPaused = false;
        public bool IsPaused
        {
            get { return _isPaused; }
            set { Set(() => IsPaused, ref _isPaused, value); }
        }

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(async () => await ViewLoaded()));

        private RelayCommand<string> _activityCommand;
        public RelayCommand<string> ActivityCommand => _activityCommand ?? (_activityCommand = new RelayCommand<string>((item) =>
        {
            ActivityRecording recordingActivity = Enum<ActivityRecording>.Parse(item);
            ActivityText = recordingActivity.ToString();
        }));

        private RelayCommand _recordCommand;
        public RelayCommand RecordCommand => _recordCommand ?? (_recordCommand = new RelayCommand(async () => await Recording(), () => _canRecord));

        private RelayCommand _pauseCommand;        
        public RelayCommand PauseCommand => _pauseCommand ?? (_pauseCommand = new RelayCommand(() => IsPaused = !IsPaused));

        private RelayCommand _stopCommand;
        public RelayCommand StopCommand => _stopCommand ?? (_stopCommand = new RelayCommand(async () => await StopRecording()));

        public RecordViewModel(INavigationService navigationService, ILocationService locationService, IGPXService gpxService) : base(navigationService)
        {
            _gpxService = gpxService;
            _locationService = locationService;            
            //_locationService.StatusChanged += OnLocationServiceStatusChanged;
        }

        //private void OnLocationServiceStatusChanged(object sender, LocationServiceStatusChangedEventArgs e)
        //{
        //}

        private async Task ViewLoaded()
        {
            _loading = true;

            //TODO: Glenn - refactor to settings option
            ActivityText = ActivityRecording.Cycling.ToString();

            //TODO: Glenn - Better the GPS tracking based on sport type and during activity
            /*
            Wondering about similar as questioned here: https://social.msdn.microsoft.com/Forums/en-US/ac03380d-4872-4161-b450-57a3c064f0a3/uwp-how-to-determine-if-a-device-has-gps-capabilities
            For creating GPS tracking we want to be sure the user can only start that if accuracy is high enough
            So wondering what the best threshold should be in meters...
            AND how 'fast' I should set the recording milliseconds

            Some suggestions already given by Atley Hunter ( and I'm also seeing this in my Garmin GPX files )
                * Tweak the ReportInterval during the tracking, to ensure better app performance
                * Set Geolocator settings depending on cycling or running
                * Strava app will show halo around current location indicating accuracy, how smaller the halo, the more accurate the position ( would guess this is the distance in meter? )
            */

            //DistanceUnitType distanceUnitType = await _settingsService.GetStoredDistanceUnitTypeAsync();
            //SelectedMeasurementUnit = distanceUnitType.ToString();

            var accessStatus = await _locationService.RequestAccessAsync();

            switch (accessStatus)
            {
                case LocationServiceRequestResult.Allowed:
                    var position = await _locationService.GetPositionAsync(LocationServiceAccuracy.High);
                    if (!position.IsUnknown)
                    {
                        _canRecord = true;
                        RecordCommand.RaiseCanExecuteChanged();
                        CurrentLocation = new Geopoint(new BasicGeoposition()
                        {
                            Latitude = position.Latitude,
                            Longitude = position.Longitude
                        });
                    }
                    break;
                case LocationServiceRequestResult.Denied:
                    //TODO: Glenn - Location request denied, show user information!
                    break;
            }

            _loading = false;
        }

        private void ClearExtendedExecution()
        {
            if (_extendedExecutionSession != null)
            {
                _extendedExecutionSession.Revoked -= OnExtendedExecutionSessionRevoked;
                _extendedExecutionSession.Dispose();
                _extendedExecutionSession = null;
            }
        }

        private void EndExtendedExecution()
        {
            IsRecording = false;
            _periodicTimer?.Dispose();
            ClearExtendedExecution();
        }

        private async Task Recording()
        {
            //The previous Extended Execution must be closed before a new one can be requested
            //TODO: Glenn - we normally have to call this clear method when navigating away from the screen!
            ClearExtendedExecution();
            ExtendedExecutionSession newSession = new ExtendedExecutionSession
            {
                Reason = ExtendedExecutionReason.LocationTracking,
                Description = "Tracking your location"
            };

            newSession.Revoked += OnExtendedExecutionSessionRevoked;
            ExtendedExecutionResult result = await newSession.RequestExtensionAsync();
            switch (result)
            {
                case ExtendedExecutionResult.Allowed:
                    //TODO: Glenn - start location tracking!
                    IsRecording = true;
                    await _gpxService.InitGPXDocument();                    
                    _periodicTimer = new Timer(OnTimer, _locationService, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2.2));
                    break;
                default:
                case ExtendedExecutionResult.Denied:
                    newSession.Dispose();
                    break;
            }
        }

        private async Task StopRecording()
        {
            //TODO: Glenn - Check if service is recording?
            await _gpxService.EndGPXDocument();            

            EndExtendedExecution();            
        }

        private async void OnExtendedExecutionSessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            await DispatcherHelper.RunAsync(() =>
            {
                switch (args.Reason)
                {
                    case ExtendedExecutionRevokedReason.Resumed:
                        //rootPage.NotifyUser("Extended execution revoked due to returning to foreground.", NotifyType.StatusMessage);
                        break;
                    case ExtendedExecutionRevokedReason.SystemPolicy:
                        //rootPage.NotifyUser("Extended execution revoked due to system policy.", NotifyType.StatusMessage);                    
                        break;
                }
                EndExtendedExecution();
            });
        }

        private async void OnTimer(object state)
        {
            await DispatcherHelper.RunAsync(async () =>
            {
                var locatorService = (ILocationService)state;
                if (locatorService != null && !IsPaused)
                {
                    var position = await _locationService.GetPositionAsync();
                    if (!position.IsUnknown)
                    {
                        await _gpxService.WriteGPXLocation(position.Latitude, position.Longitude);
                        CurrentLocation = new Geopoint(new BasicGeoposition()
                        {
                            Latitude = position.Latitude,
                            Longitude = position.Longitude
                        });
                    }
                }
            });
        }
    }
}
