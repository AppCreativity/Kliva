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
using Kliva.Extensions;
using Kliva.Messages;
using Kliva.Models;
using Kliva.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace Kliva.Controls
{
    public sealed partial class ActivityDetailControl : UserControl
    {
        //public IStravaViewModel ViewModel => DataContext as IStravaViewModel;
        public ActivityDetailViewModel ViewModel => DataContext as ActivityDetailViewModel;

        private readonly Dictionary<ActivityPivots, Tuple<int, PivotItem>> _pivotDictionary = new Dictionary<ActivityPivots, Tuple<int, PivotItem>>();

        public ActivityDetailControl()
        {
            InitializeComponent();
            ActivityMap.MapServiceToken = StravaIdentityConstants.MAPS_SERVICETOKEN;

            //DataContextChanged += (sender, arg) => this.Bindings.Update();

            ServiceLocator.Current.GetInstance<IMessenger>().Register<PolylineMessage>(this, Tokens.ActivityPolylineMessage, async message => await DrawPolyline(message.Geopositions));
            ServiceLocator.Current.GetInstance<IMessenger>().Register<PivotMessage<ActivityPivots>>(this, Tokens.ActivityPivotMessage, AdjustPivots);
        }

        private void AdjustPivots(PivotMessage<ActivityPivots> message)
        {
            foreach (PivotItem item in ActivityPivot.Items.ToList())
            {
                if (item.Visibility == Visibility.Collapsed)
                    ActivityPivot.Items.Remove(item);
            }

            if (!ReferenceEquals(message, null) && message.Visible)
            {               
                //Handle Defer Loaded pivots
                if (!_pivotDictionary.ContainsKey(message.Pivot))
                {
                    //Realize UI element
                    FindName($"{message.Pivot.ToString()}Pivot");
                    //Reindex collection
                    _pivotDictionary.Clear();
                    IndexPivotCollection();
                }
                else
                {
                    Tuple<int, PivotItem> pivotItem = _pivotDictionary[message.Pivot];

                    if (!ActivityPivot.Items.Contains(pivotItem.Item2))
                        ActivityPivot.Items.Insert(pivotItem.Item1, pivotItem.Item2);
                }
            }

            if (message.Show.HasValue && message.Show.Value)
                ActivityPivot.SelectedIndex = ActivityPivot.Items.IndexOf(_pivotDictionary[message.Pivot].Item2);
        }

        private async Task DrawPolyline(List<BasicGeoposition> geopositions)
        {
            ActivityMap.MapElements.Clear();

            if (geopositions.Any())
            {
                if (ExpandMapButton == null)
                    FindName("ExpandMapButton");
                else
                    ExpandMapButton.Visibility = Visibility.Visible;

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
            else
                if (ExpandMapButton != null)
                    ExpandMapButton.Visibility = Visibility.Collapsed;
        }

        private void OnActivityDetailControlLoaded(object sender, RoutedEventArgs e)
        {
            if (_pivotDictionary.Count == 0)
                IndexPivotCollection();
        }

        private void IndexPivotCollection()
        {
            int pivotIndex = 0;
            foreach (PivotItem item in ActivityPivot.Items.ToList())
            {
                _pivotDictionary.Add(Enum<ActivityPivots>.Parse((string) item.Tag), Tuple.Create(pivotIndex, item));
                ++pivotIndex;
            }
        }
    }
}