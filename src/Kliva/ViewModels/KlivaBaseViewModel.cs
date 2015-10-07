using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight;

namespace Kliva.ViewModels
{
    public class KlivaBaseViewModel : ViewModelBase
    {
        protected INavigationService _navigationService;

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set<bool>(() => IsBusy, ref _isBusy, value); }
        }

        public KlivaBaseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;            
        }
    }
}
