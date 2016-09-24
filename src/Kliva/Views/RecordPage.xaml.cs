using Cimbalino.Toolkit.Extensions;
using Kliva.Controls;
using Kliva.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;

namespace Kliva.Views
{
    public sealed partial class RecordPage : Page
    {
        private readonly MapIcon _currentLocation = new MapIcon();
        private readonly MapPolyline _mapPolyline = new MapPolyline();

        private RecordViewModel ViewModel => DataContext as RecordViewModel;

        public RecordPage()
        {
            this.InitializeComponent();
            //TODO: Glenn - Get a bigger icon?
            _currentLocation.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/CurrentPosition.png"));
            _currentLocation.NormalizedAnchorPoint = new Point(0.5, 0.5);
            _currentLocation.ZIndex = 0;

            _mapPolyline.StrokeColor = (Color)Application.Current.Resources["KlivaMainColor"];
            _mapPolyline.StrokeThickness = 3;

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Cleanup();
            base.OnNavigatedFrom(e);
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(ViewModel.CurrentLocation), StringComparison.OrdinalIgnoreCase))
            {
                //ToDo PiNi: only do this the first time?
                //and add button to re-center map?
                //==> To discuss with team
                CenterMap();

                UpdateMapElements();

                if(!TrackingMap.MapElements.Contains(_currentLocation))
                    TrackingMap.MapElements.Add(_currentLocation);

                if (!TrackingMap.MapElements.Contains(_mapPolyline))
                    TrackingMap.MapElements.Add(_mapPolyline);
            }
        }

        private void UpdateMapElements()
        {
            List<BasicGeoposition> positions;

            if(_mapPolyline.Path?.Positions != null)
                positions = new List<BasicGeoposition>(_mapPolyline.Path.Positions);
            else
                positions = new List<BasicGeoposition>();

            positions.Add(ViewModel.CurrentLocation.Position);
            _mapPolyline.Path = new Geopath(positions);

            _currentLocation.Location = ViewModel.CurrentLocation;
        }

        private void CenterMap()
        {
            TrackingMap.Center = ViewModel.CurrentLocation;
            TrackingMap.ZoomLevel = 16.0;
        }

        /// <summary>
        /// Set menu flyout width equal to the container it's in
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnActivityFlyoutOpened(object sender, object e)
        {
            MenuFlyout menuFlyout = sender as MenuFlyout;
            Style style = new Style { TargetType = typeof(MenuFlyoutPresenter) };
            style.Setters.Add(new Setter(MinWidthProperty, ContentPanel.ActualWidth));

            //TODO: Glenn - Can't we just grab : {ThemeResource SplitViewCompactPaneThemeLength} - we can but than it could be different from the actual size during runtime
            //TODO: Convert.ToInt32((Application.Current.Resources["SplitViewCompactPaneThemeLength"] as double?).Value)
            SplitView splitView = ((KlivaApplicationFrame)this.Parent).GetVisualDescendents<SplitView>().FirstOrDefault();
            if (splitView.DisplayMode != SplitViewDisplayMode.Inline)
                style.Setters.Add(new Setter(MarginProperty, new Thickness(splitView.CompactPaneLength, 0, 0, 0)));

            menuFlyout.MenuFlyoutPresenterStyle = style;
        }
    }
}
