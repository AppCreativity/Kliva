using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using GalaSoft.MvvmLight.Messaging;
using Kliva.Messages;
using Kliva.Models;
using Kliva.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace Kliva.Views
{
    public sealed partial class SegmentPage : Page
    {
        private SegmentViewModel ViewModel => DataContext as SegmentViewModel;

        public SegmentPage()
        {
            InitializeComponent();
            SegmentMap.MapServiceToken = StravaIdentityConstants.MAPS_SERVICETOKEN;

            ServiceLocator.Current.GetInstance<IMessenger>().Register<PolylineMessage>(this, Tokens.SegmentPolylineMessage, async message => await DrawPolyline(message.Geopositions));
        }

        private async Task DrawPolyline(List<BasicGeoposition> geopositions)
        {
            SegmentMap.MapElements.Clear();

            if (geopositions.Any())
            {
                if (ExpandMapButton == null)
                    FindName("ExpandMapButton");
                else
                    ExpandMapButton.Visibility = Visibility.Visible;

                var polyLine = new MapPolyline { Path = new Geopath(geopositions), StrokeThickness = 4, StrokeColor = (Color)App.Current.Resources["StravaRedColor"] };
                SegmentMap.MapElements.Add(polyLine);

                MapIcon startMapIcon = new MapIcon();
                startMapIcon.Location = new Geopoint(geopositions.First());
                startMapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                startMapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Start.png"));
                SegmentMap.MapElements.Add(startMapIcon);

                MapIcon endMapIcon = new MapIcon();
                endMapIcon.Location = new Geopoint(geopositions.Last());
                endMapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                endMapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/End.png"));
                SegmentMap.MapElements.Add(endMapIcon);

                var zoomed = false;
                while (!zoomed)
                    zoomed = await SegmentMap.TrySetViewBoundsAsync(GeoboundingBox.TryCompute(geopositions), null, MapAnimationKind.None);
            }
            else
                if (ExpandMapButton != null)
                ExpandMapButton.Visibility = Visibility.Collapsed;
        }
    }
}
