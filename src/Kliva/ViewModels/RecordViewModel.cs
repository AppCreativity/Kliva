using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Models;

namespace Kliva.ViewModels
{
    public class RecordViewModel : KlivaBaseViewModel
    {
        private bool _loading;
        private readonly ILocationService _locationService;

        private Geopoint _currentLocation;
        public Geopoint CurrentLocation
        {
            get { return _currentLocation; }
            set { Set(() => CurrentLocation, ref _currentLocation, value); }
        }

        private RelayCommand _viewLoadedCommand;
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(async () => await ViewLoaded()));

        public RecordViewModel(INavigationService navigationService, ILocationService locationService) : base(navigationService)
        {
            _locationService = locationService;

            //Set the default interval to 3 seconds
            _locationService.ReportInterval = 3000;
        }

        private async Task ViewLoaded()
        {
            _loading = true;

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
    }
}
