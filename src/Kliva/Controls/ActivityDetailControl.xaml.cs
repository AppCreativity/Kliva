using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using GalaSoft.MvvmLight.Messaging;
using Kliva.Messages;
using Kliva.ViewModels;
using Kliva.ViewModels.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace Kliva.Controls
{
    public sealed partial class ActivityDetailControl : UserControl
    {
        //public IStravaViewModel ViewModel => DataContext as IStravaViewModel;
        public ActivityDetailViewModel ViewModel => DataContext as ActivityDetailViewModel;

        public ActivityDetailControl()
        {
            this.InitializeComponent();
            //DataContextChanged += (sender, arg) => this.Bindings.Update();

            ServiceLocator.Current.GetInstance<IMessenger>().Register<ActivityPolylineMessage>(this, async message => await this.DrawPolyline(message.Geopositions));
        }

        private async Task DrawPolyline(List<BasicGeoposition> geopositions)
        {
            ActivityMap.MapElements.Clear();

            if (geopositions.Any())
            {
                var polyLine = new MapPolyline { Path = new Geopath(geopositions), StrokeThickness = 4, StrokeColor = (Color)App.Current.Resources["StravaRedColor"] };
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
    }
}