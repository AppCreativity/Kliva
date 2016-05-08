using System.Threading.Tasks;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Models;
using Kliva.Services.Interfaces;

namespace Kliva.ViewModels
{
    public class SegmentViewModel : KlivaBaseViewModel
    {
        private readonly IStravaService _stravaService;

        private string _currentSegmentId;

        private Segment _segment;
        public Segment Segment
        {
            get { return _segment;}
            set { Set(() => Segment, ref _segment, value); }
        }

        private RelayCommand _viewLoadedCommand;       
        public RelayCommand ViewLoadedCommand => _viewLoadedCommand ?? (_viewLoadedCommand = new RelayCommand(() => ViewLoaded()));

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
                await _stravaService.GetSegmentAsync(currentParameter);
            }
        }

        private void ClearProperties()
        {
            Segment = null;
        }
    }
}
