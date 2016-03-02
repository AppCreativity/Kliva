using Windows.UI.Xaml.Controls;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight;
using Kliva.Models;
using Kliva.Views;

namespace Kliva.ViewModels
{
    public class KlivaBaseViewModel : ViewModelBase
    {
        protected INavigationService NavigationService;

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { Set<bool>(() => IsBusy, ref _isBusy, value); }
        }

        public KlivaBaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;            
        }

        protected void OnAthleteTapped(ItemClickEventArgs args)
        {
            AthleteSummary athlete = args.ClickedItem as AthleteSummary;
            if(athlete != null)
                NavigationService.Navigate<ProfilePage>(athlete.Id.ToString());
        }
    }
}
