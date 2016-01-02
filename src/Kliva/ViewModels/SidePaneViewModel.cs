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
                        if (string.IsNullOrEmpty(SelectedBottomMenuItem.Title))
                            HamburgerCommand.Execute(null);

                        if (SelectedBottomMenuItem.Title.Equals("settings", StringComparison.OrdinalIgnoreCase))
                            SettingsCommand.Execute(null);

                        SelectedBottomMenuItem = null;
                    }
                }                
            }
        }

        private RelayCommand _hamburgerCommand;
        public RelayCommand HamburgerCommand => _hamburgerCommand ?? (_hamburgerCommand = new RelayCommand(() => this.IsPaneOpen = !this.IsPaneOpen));

        private RelayCommand _settingsCommand;
        public RelayCommand SettingsCommand => _settingsCommand ?? (_settingsCommand = new RelayCommand(() => _navigationService.Navigate<SettingsPage>()));

        public SidePaneViewModel(INavigationService navigationService) : base(navigationService)
        {
            var view = ApplicationView.GetForCurrentView();
            view.VisibleBoundsChanged += OnVisibleBoundsChanged;

            TopMenuItems.Add(new MenuItem() { Icon = "", Title = "statistics", MenuItemType = MenuItemType.MDL2 });
            TopMenuItems.Add(new MenuItem() { Icon = "", Title = "profile", MenuItemType = MenuItemType.MDL2 });
            TopMenuItems.Add(new MenuItem() { Icon = "", Title = "club", MenuItemType = MenuItemType.Material });

            BottomMenuItems.Add(new MenuItem() { Icon = "", Title = "settings", MenuItemType = MenuItemType.MDL2 });
            BottomMenuItems.Add(new MenuItem() { Icon = "", Title = "", MenuItemType = MenuItemType.MDL2 });
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
