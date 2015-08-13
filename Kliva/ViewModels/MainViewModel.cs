using System;
using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;

namespace Kliva.ViewModels
{
    public class MainViewModel : KlivaBaseViewModel
    {
        private RelayCommand _testCommand;
        public RelayCommand TestCommand
        {
            get
            {
                return _testCommand ?? (_testCommand = new RelayCommand(() => Navigate()));
            }
        }

        private void Navigate()
        {
            _navigationService.Navigate<LoginPage>();
        }

        public MainViewModel(INavigationService navigationService) : base(navigationService)
        {

        }
    }
}
