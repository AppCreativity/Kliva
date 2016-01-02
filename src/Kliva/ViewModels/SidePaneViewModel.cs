using Cimbalino.Toolkit.Services;
using GalaSoft.MvvmLight.Command;
using Kliva.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Kliva.Models;

namespace Kliva.ViewModels
{
    public class SidePaneViewModel : KlivaBaseViewModel
    {
        private readonly List<Type> _noSidePane = new List<Type>
        {
            typeof(LoginPage),
            typeof(SettingsPage)
        };

        private Type _pageType;

        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set { Set(() => IsPaneOpen, ref _isPaneOpen, value); }
        }

        private SplitViewDisplayMode _displayMode = SplitViewDisplayMode.CompactOverlay;
        public SplitViewDisplayMode DisplayMode
        {
            get { return _displayMode; }
            set { Set(() => DisplayMode, ref _displayMode, value); }
        }

        private ObservableCollection<MenuItem> _topMenuItems = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> TopMenuItems
        {
            get { return _topMenuItems; }
            set { Set(() => TopMenuItems, ref _topMenuItems, value); }
        }

        private ObservableCollection<MenuItem> _bottomMenuItems = new ObservableCollection<MenuItem>();
        public ObservableCollection<MenuItem> BottomMenuItems
        {
            get { return _bottomMenuItems; }
            set { Set(() => BottomMenuItems, ref _bottomMenuItems, value); }
        }

        private MenuItem _selectedMenuItem;
        public MenuItem SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set { Set(() => SelectedMenuItem, ref _selectedMenuItem, value); }
        }

        private RelayCommand _hamburgerCommand;
        public RelayCommand HamburgerCommand => _hamburgerCommand ?? (_hamburgerCommand = new RelayCommand(() => this.IsPaneOpen = !this.IsPaneOpen));

        private RelayCommand _settingsCommand;
        public RelayCommand SettingsCommand => _settingsCommand ?? (_settingsCommand = new RelayCommand(() => _navigationService.Navigate<SettingsPage>()));

        public SidePaneViewModel(INavigationService navigationService) : base(navigationService)
        {
            var view = ApplicationView.GetForCurrentView();
            view.VisibleBoundsChanged += OnVisibleBoundsChanged;

            TopMenuItems.Add(new MenuItem() {Icon = "", Title = "statistics"});
            TopMenuItems.Add(new MenuItem() {Icon = "", Title = "profile"});

            BottomMenuItems.Add(new MenuItem() { Icon = "", Title = "settings" });
            BottomMenuItems.Add(new MenuItem() { Icon = "", Title = "" });
        }

        private void OnVisibleBoundsChanged(ApplicationView sender, object args)
        {
            this.ShowHide();
        }

        internal void ShowHide(Type pageType = null)
        {
            //Set current pageType
            if (pageType != null)
                _pageType = pageType;

            bool show = true;

            //TODO: Glenn - Verify this solution with these remarks http://stackoverflow.com/questions/31936154/get-screen-resolution-in-win10-uwp-app
            var actualWidth = ApplicationView.GetForCurrentView().VisibleBounds.Width;
            if (actualWidth < 720)
                show = false;
            else
                show = !_noSidePane.Contains(_pageType);

            if (show)
                this.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            else
            {
                this.DisplayMode = SplitViewDisplayMode.Inline;
                this.IsPaneOpen = false;
            }
        }
    }
}
