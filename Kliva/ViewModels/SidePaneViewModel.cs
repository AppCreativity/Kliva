using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kliva.Models;
using Kliva.Views;
using Windows.UI.Xaml.Controls;

namespace Kliva.ViewModels
{
    public class SidePaneViewModel : KlivaBaseViewModel
    {
        private bool _isPaneOpen = true;
        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set { Set(() => IsPaneOpen, ref _isPaneOpen, value); }
        }

        private bool _isPaneVisible = true;
        public bool IsPaneVisible
        {
            get { return _isPaneVisible; }
            set { Set(() => IsPaneVisible, ref _isPaneVisible, value); }
        }

        private SplitViewDisplayMode _displayMode = SplitViewDisplayMode.CompactOverlay;
        public SplitViewDisplayMode DisplayMode
        {
            get { return _displayMode; }
            set { Set(() => DisplayMode, ref _displayMode, value); }
        }

        private RelayCommand _hamburgerCommand;
        public RelayCommand HamburgerCommand => _hamburgerCommand ?? (_hamburgerCommand = new RelayCommand(() => this.IsPaneOpen = !this.IsPaneOpen));

        private RelayCommand _settingsCommand;
        public RelayCommand SettingsCommand => _settingsCommand ?? (_settingsCommand = new RelayCommand(() => _navigationService.Navigate<SettingsPage>()));

        public SidePaneViewModel(INavigationService navigationService) : base(navigationService)
        {

        }

        internal void ShowHide(bool show)
        {
            if (show)
                this.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            else
                this.DisplayMode = SplitViewDisplayMode.Inline;

            this.IsPaneVisible = show;
        }
    }
}
