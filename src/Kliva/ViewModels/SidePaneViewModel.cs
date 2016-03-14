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
            typeof(LoginPage)
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

        private MenuItem _selectedTopMenuItem;
        public MenuItem SelectedTopMenuItem
        {
            get { return _selectedTopMenuItem; }
            set
            {
                if (Set(() => SelectedTopMenuItem, ref _selectedTopMenuItem, value))
                {
                    if (value != null)
                    {
                        if (this.IsPaneOpen)
                            this.IsPaneOpen = !this.IsPaneOpen;

                        switch (value.MenuItemType)
                        {
                            case MenuItemType.Home:
                                HomeCommand.Execute(null);
                                break;

                            case MenuItemType.Statistics:
                                StatisticsCommand.Execute(null);
                                break;

                            case MenuItemType.Profile:
                                ProfileCommand.Execute(null);
                                break;

                            case MenuItemType.Clubs:
                                ClubsCommand.Execute(null);
                                break;
                        }

                        SelectedTopMenuItem = null;
                    }
                }
            }
        }

        private MenuItem _selectedBottomMenuItem;
        public MenuItem SelectedBottomMenuItem
        {
            get { return _selectedBottomMenuItem; }
            set
            {
                if (Set(() => SelectedBottomMenuItem, ref _selectedBottomMenuItem, value))
                {
                    if (value != null)
                    {
                        switch (value.MenuItemType)
                        {
                            case MenuItemType.Settings:
                                SettingsCommand.Execute(null);
                                break;
                            case MenuItemType.Empty:
                                HamburgerCommand.Execute(null);
                                break;
                        }

                        SelectedBottomMenuItem = null;
                    }
                }                
            }
        }

        private RelayCommand _homeCommand;
        public RelayCommand HomeCommand => _homeCommand ?? (_homeCommand = new RelayCommand(() => ChangePage<MainPage>()));

        //TODO: Glenn - We hooked this up twice, once in SidePaneViewModel and once in MainViewModel because of difference in UI on desktop ( sidebar ) and mobile ( bottom appbar )
        private RelayCommand _statisticsCommand;
        public RelayCommand StatisticsCommand => _statisticsCommand ?? (_statisticsCommand = new RelayCommand(() => NavigationService.Navigate<StatsPage>()));

        private RelayCommand _profileCommand;
        public RelayCommand ProfileCommand => _profileCommand ?? (_profileCommand = new RelayCommand(() => NavigationService.Navigate<ProfilePage>()));

        private RelayCommand _clubsCommand;
        public RelayCommand ClubsCommand => _clubsCommand ?? (_clubsCommand = new RelayCommand(() => ChangePage<ClubsPage>()));

        private RelayCommand _hamburgerCommand;
        public RelayCommand HamburgerCommand => _hamburgerCommand ?? (_hamburgerCommand = new RelayCommand(() => this.IsPaneOpen = !this.IsPaneOpen));

        //TODO: Glenn - We hooked this up twice, once in SidePaneViewModel and once in MainViewModel because of difference in UI on desktop ( sidebar ) and mobile ( bottom appbar )
        private RelayCommand _settingsCommand;
        public RelayCommand SettingsCommand => _settingsCommand ?? (_settingsCommand = new RelayCommand(() => ChangePage<SettingsPage>()));

        public SidePaneViewModel(INavigationService navigationService) : base(navigationService)
        {
            if (!IsInDesignMode)
            {
                var view = ApplicationView.GetForCurrentView();
                view.VisibleBoundsChanged += OnVisibleBoundsChanged;
            }

            TopMenuItems.Add(new MenuItem() { Icon = "", Title = "home", MenuItemType = MenuItemType.Home, MenuItemFontType = MenuItemFontType.MDL2 });
            //TopMenuItems.Add(new MenuItem() { Icon = "", Title = "statistics", MenuItemType = MenuItemType.Statistics, MenuItemFontType = MenuItemFontType.MDL2 });
            TopMenuItems.Add(new MenuItem() { Icon = "", Title = "statistics", MenuItemType = MenuItemType.Statistics, MenuItemFontType = MenuItemFontType.Material });
            TopMenuItems.Add(new MenuItem() { Icon = "", Title = "profile", MenuItemType = MenuItemType.Profile, MenuItemFontType = MenuItemFontType.MDL2 });
            TopMenuItems.Add(new MenuItem() { Icon = "", Title = "club", MenuItemType = MenuItemType.Clubs, MenuItemFontType = MenuItemFontType.Material });

            BottomMenuItems.Add(new MenuItem() { Icon = "", Title = "settings", MenuItemType = MenuItemType.Settings, MenuItemFontType = MenuItemFontType.MDL2 });
            BottomMenuItems.Add(new MenuItem() { Icon = "", Title = "", MenuItemType = MenuItemType.Empty, MenuItemFontType = MenuItemFontType.MDL2 });
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

        private void ChangePage<DestinationPageType>()
        {
            // The side pane does not pass a navigation parameter, we can use this to distinguish
            // between a top-level page versus some other page in the hierarchy
            if (typeof(DestinationPageType) != _pageType && NavigationService.CurrentParameter == null)
            {
                NavigationService.Navigate<DestinationPageType>();
            }
        }
    }
}
