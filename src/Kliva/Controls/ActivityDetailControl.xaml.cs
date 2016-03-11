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
using Kliva.Helpers;
using Kliva.Messages;
using Kliva.Models;
using Kliva.ViewModels;
using ImplicitAnimations;
using Microsoft.Practices.ServiceLocation;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Composition;
using System.Diagnostics;
using CompositionSampleGallery;

namespace Kliva.Controls
{
    public sealed partial class ActivityDetailControl : UserControl
    {
        //public IStravaViewModel ViewModel => DataContext as IStravaViewModel;
        public ActivityDetailViewModel ViewModel => DataContext as ActivityDetailViewModel;

        private readonly Dictionary<Pivots, Tuple<int, PivotItem>> _pivotDictionary = new Dictionary<Pivots, Tuple<int, PivotItem>>();

        public ActivityDetailControl()
        {
            this.InitializeComponent();

            ActivityMap.EnableLayoutImplicitAnimations();



            //DataContextChanged += (sender, arg) => this.Bindings.Update();

            ServiceLocator.Current.GetInstance<IMessenger>().Register<ActivityPolylineMessage>(this, async message => await DrawPolyline(message.Geopositions));
            ServiceLocator.Current.GetInstance<IMessenger>().Register<PivotMessage>(this, AdjustPivots);
        }

        private void AdjustPivots(PivotMessage message)
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

                //Make room for the glass

                Thickness padding; 
                if (VisualStateGroup.CurrentState.Name=="Mobile") {
                     padding = new Thickness(0, 190, 0, 0);
                }
                
                while (!zoomed)
                {
                    zoomed = await ActivityMap.TrySetViewBoundsAsync(GeoboundingBox.TryCompute(geopositions), padding, MapAnimationKind.None);
                }
            }
            else
                if (ExpandMapButton != null)
                ExpandMapButton.Visibility = Visibility.Collapsed;
        }

        private void OnActivityDetailControlLoaded(object sender, RoutedEventArgs e)
        {
            //ActivityMap.Opacity = 1.0f;

            var scrollerManipProps = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scroller);

            Compositor compositor = scrollerManipProps.Compositor;

            if (VisualStateGroup.CurrentState.Name == "Mobile")
            {
                // Experimental custom scrolling

                // Create the expression 
                ExpressionAnimation expression = compositor.CreateExpressionAnimation("scroller.Translation.Y");

                // set "dynamic" reference parameter that will be used to evaluate the current position of the scrollbar every frame 
                expression.SetReferenceParameter("scroller", scrollerManipProps);

                // Get the background image and start animating it's offset using the expression 
                Visual backgroundVisual = ElementCompositionPreview.GetElementVisual(BlurPanel);
                Visual mapVisual = ElementCompositionPreview.GetElementVisual(ActivityMap);
                Visual pivotVisual = ElementCompositionPreview.GetElementVisual(ActivityPivot);
                backgroundVisual.StartAnimation("Offset.Y", expression);
                mapVisual.StartAnimation("Offset.Y", expression);
                pivotVisual.StartAnimation("Offset.Y", expression);
            }

            if (_pivotDictionary.Count == 0)
                IndexPivotCollection();
        }

        private void IndexPivotCollection()
        {
            int pivotIndex = 0;
            foreach (PivotItem item in ActivityPivot.Items.ToList())
            {
                _pivotDictionary.Add(Enum<Pivots>.Parse((string)item.Tag), Tuple.Create(pivotIndex, item));
                ++pivotIndex;
            }
        }

        private void ActivityPhotosGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            Func<object, Uri> getImageForPhoto = (o) => { return new Uri(((Photo)o).ImageLarge); };

            //Display the imageViewer as an app modal experience.  Margin determines how much of the app is visible around the edges of the dialog
            ImagePopupViewer.Show(ActivityPhotosGrid.ItemsSource, getImageForPhoto, new Thickness(100, 50, 50, 50));
        }
    }
}