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
using Kliva.Models;

namespace Kliva.Views
{
    public sealed partial class MapPage : Page
    {
        public MapPage()
        {
            InitializeComponent();
            ActivityMap.MapServiceToken = StravaIdentityConstants.MAPS_SERVICETOKEN;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e?.Parameter is Map)
                DrawPolyline(((Map)e.Parameter).GeoPositions);            
        }

        private async void DrawPolyline(List<BasicGeoposition> geopositions)
        {
            ActivityMap.MapElements.Clear();

            if (geopositions.Any())
            {
                var polyLine = new MapPolyline { Path = new Geopath(geopositions), StrokeThickness = 4, StrokeColor = (Color)Application.Current.Resources["StravaRedColor"] };
                ActivityMap.MapElements.Add(polyLine);

                MapIcon startMapIcon = new MapIcon();
                startMapIcon.Location = new Geopoint(geopositions.First());
                startMapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                startMapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Start.png"));
                ActivityMap.MapElements.Add(startMapIcon);

                MapIcon endMapIcon = new MapIcon();
                endMapIcon.Location = new Geopoint(geopositions.Last());
                endMapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                endMapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/End.png"));
                ActivityMap.MapElements.Add(endMapIcon);

                var zoomed = false;
                while (!zoomed)
                    zoomed = await ActivityMap.TrySetViewBoundsAsync(GeoboundingBox.TryCompute(geopositions), null, MapAnimationKind.None);
            }
        }

        private void OnToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            ActivityMap.Style = MapStyle.AerialWithRoads;
        }

        private void OnToggleButtonUnchecked(object sender, RoutedEventArgs e)
        {
            ActivityMap.Style = MapStyle.Road;
        }
    }
}
