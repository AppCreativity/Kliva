using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Kliva.Messages;
using Kliva.Models;
using Kliva.Services.Interfaces;
using Kliva.Views;
using Microsoft.Practices.ServiceLocation;

namespace Kliva.ViewModels
{
    public class SegmentViewModel : KlivaBaseViewModel
    {
        private readonly IStravaService _stravaService;

        private string _currentSegmentId;

        public Segment Segment { get; set; }

        private SegmentEffort _segmentEffort;
        public SegmentEffort SegmentEffort
        {
            get { return _segmentEffort;}
            set { Set(() => SegmentEffort, ref _segmentEffort, value); }
        }

        private RelayCommand _viewLoadedCommand;       
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(() => ViewLoaded()));

        private RelayCommand _mapCommand;
        public RelayCommand MapCommand => _mapCommand ?? (_mapCommand = new RelayCommand(() => NavigationService.Navigate<MapPage>(Segment?.Map)));

        public SegmentViewModel(INavigationService navigationService, IStravaService stravaService) : base(navigationService)
        {
            _stravaService = stravaService;
        }

        private Task ViewLoaded()
        {
            if (string.IsNullOrEmpty(_currentSegmentId) || !string.Equals(_currentSegmentId, NavigationService.CurrentParameter?.ToString()))
                return LoadAsync();

            return Task.CompletedTask;
        }

        private async Task LoadAsync()
        {
            //TODO: All - configure ioc to give a new VM per call?
            ClearProperties();

            string currentParameter = (string)NavigationService.CurrentParameter;
            if (!string.IsNullOrEmpty(currentParameter))
            {
                //TODO: Glenn - What do we need? Segment of Segment Effort or both?
                //TODO: Glenn - We need Segment Effort for analytics! So move analytics groups to SegmentEffortClass
                SegmentEffort = await _stravaService.GetSegmentEffortAsync(currentParameter);
                //TODO: Glenn - We need to retrieve the actual segment too, for extra data ( like MAP ) - Look how we can combine/merge this with SegmentEffort.Segment
                Segment = await _stravaService.GetSegmentAsync(SegmentEffort.Segment.Id.ToString());

                ServiceLocator.Current.GetInstance<IMessenger>()
                    .Send<PolylineMessage>(Segment.Map.GeoPositions.Any()
                        ? new PolylineMessage(Segment.Map.GeoPositions)
                        : new PolylineMessage(new List<BasicGeoposition>()), Tokens.SegmentPolylineMessage);
            }
        }

        

        private void ClearProperties()
        {
            SegmentEffort = null;
        }
    }
}
