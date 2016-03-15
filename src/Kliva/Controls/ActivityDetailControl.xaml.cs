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
                ExpressionAnimation expression = compositor.CreateExpressionAnimation();

                // set "dynamic" reference parameter that will be used to evaluate the current position of the scrollbar every frame 
                expression.SetReferenceParameter("scroller", scrollerManipProps);
                expression.Expression = "Clamp(scroller.Translation.Y,-200,0)";

                Visual pivotVisual = ElementCompositionPreview.GetElementVisual(ActivityPivot);
                pivotVisual.StartAnimation("Offset.Y", expression);

                Visual mapVisual = ElementCompositionPreview.GetElementVisual(ActivityMap);
                mapVisual.StartAnimation("Offset.Y", expression);

                ExpressionAnimation headerColapseExpression = compositor.CreateExpressionAnimation();
                headerColapseExpression.SetReferenceParameter("scroller", scrollerManipProps);
                headerColapseExpression.SetScalarParameter("minClamp", (float)BlurPanel.ActualHeight - 50); //50 is the desired height of the header when collapsed
                //mapMoveExpression.Expression = "Clamp(scroller.Translation.Y, minClamp, 0)";

                headerColapseExpression.Expression = "Clamp(scroller.Translation.Y,-130,0)";
                
                Visual backgroundVisual = ElementCompositionPreview.GetElementVisual(BlurPanel);
                backgroundVisual.StartAnimation("Offset.Y", headerColapseExpression);

                ExpressionAnimation fadeDetails = compositor.CreateExpressionAnimation();
                fadeDetails.SetReferenceParameter("scroller", scrollerManipProps);
                fadeDetails.Expression = "1-(Clamp(scroller.Translation.Y, -80, 0)/-80)";  //80 is number of pixels scrolled by which the element will be at 0 opacity 
                Visual profilePictureVideo = ElementCompositionPreview.GetElementVisual(AthleteProfilePicture);
                profilePictureVideo.StartAnimation("Opacity", fadeDetails);

                ElementCompositionPreview.GetElementVisual(BikeIcon).StartAnimation("Opacity", fadeDetails);
                
                ElementCompositionPreview.GetElementVisual(ClockIcon).StartAnimation("Opacity", fadeDetails);

                ElementCompositionPreview.GetElementVisual(TimeMovingFormattedTextBlockHeader).StartAnimation("Opacity", fadeDetails);

                ElementCompositionPreview.GetElementVisual(DistanceFormattedTextBlockHeader).StartAnimation("Opacity", fadeDetails);

                ExpressionAnimation moveTitle = compositor.CreateExpressionAnimation();
                moveTitle.SetReferenceParameter("scroller", scrollerManipProps);
                moveTitle.Expression = "Clamp(scroller.Translation.Y*0.2, -20, 0)";  //100 is number of pixels scrolled by which the element will be at 0 opacity 
                Visual activityName = ElementCompositionPreview.GetElementVisual(ActivityName);
                activityName.StartAnimation("Offset.Y", moveTitle);
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