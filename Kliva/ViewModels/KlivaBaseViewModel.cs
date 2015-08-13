using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight;

namespace Kliva.ViewModels
{
    public class KlivaBaseViewModel : ViewModelBase
    {
        protected INavigationService _navigationService;

        public KlivaBaseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}
