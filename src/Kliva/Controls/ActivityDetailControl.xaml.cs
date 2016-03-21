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
using SamplesCommon;

namespace Kliva.Controls
{
    public sealed partial class ActivityDetailControl : UserControl
    {
        //public IStravaViewModel ViewModel => DataContext as IStravaViewModel;
        public ActivityDetailViewModel ViewModel => DataContext as ActivityDetailViewModel;

        private readonly Dictionary<Pivots, Tuple<int, PivotItem>> _pivotDictionary = new Dictionary<Pivots, Tuple<int, PivotItem>>();

        private ExpressionAnimation m_headerColapseExpression;
        private Visual m_blurPanelVisual;

        public ActivityDetailControl()
        {
            this.InitializeComponent();

            ActivityMap.EnableLayoutImplicitAnimations();

            ServiceLocator.Current.GetInstance<IMessenger>().Register<ActivityPolylineMessage>(this, async message => await DrawPolyline(message.Geopositions));
            ServiceLocator.Current.GetInstance<IMessenger>().Register<PivotMessage>(this, AdjustPivots);
            Loading += OnLoading;
        }

        private void OnLoading(FrameworkElement sender, object args)
        {
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (m_headerColapseExpression != null)
            {
                m_headerColapseExpression.SetScalarParameter("minHeight", (float)BlurPanel.ActualHeight - 50); //50 is the desired height of the header when collapsed
                m_blurPanelVisual.StartAnimation("Offset.Y", m_headerColapseExpression);
            }
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
            var scrollerManipProps = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scroller);

            Compositor compositor = scrollerManipProps.Compositor;

            if (VisualStateGroup.CurrentState.Name == "Mobile")
            {
                SetupShyHeaderExpressions(scrollerManipProps, compositor);
            }

            if (_pivotDictionary.Count == 0)
                IndexPivotCollection();
        }

        private void SetupShyHeaderExpressions(CompositionPropertySet scrollerManipProps, Compositor compositor)
        {
            #region MoveMap
            //// Create the expression 
            ExpressionAnimation expression = compositor.CreateExpressionAnimation();

            // set "dynamic" reference parameter that will be used to evaluate the current position of the scrollbar every frame 
            expression.SetReferenceParameter("scroller", scrollerManipProps);

            // move the target up the screen a maximum of 200 pixels
            expression.Expression = "Clamp(scroller.Translation.Y,-200,0)";

            ElementCompositionPreview.GetElementVisual(ActivityPivot).StartAnimation("Offset.Y", expression);
            ElementCompositionPreview.GetElementVisual(ActivityMap).StartAnimation("Offset.Y", expression);
            #endregion

            #region FadeDetails
            ExpressionAnimation fadeDetails = compositor.CreateExpressionAnimation();
            fadeDetails.SetReferenceParameter("scroller", scrollerManipProps);
            
            //80 is number of pixels scrolled by which the element will be at 0 opacity 
            fadeDetails.Expression = "1-(Clamp(scroller.Translation.Y, -80, 0)/-80)";  
            Visual profilePictureVideo = ElementCompositionPreview.GetElementVisual(AthleteProfilePicture);
            profilePictureVideo.StartAnimation("Opacity", fadeDetails);

            ElementCompositionPreview.GetElementVisual(BikeIcon).StartAnimation("Opacity", fadeDetails);
            ElementCompositionPreview.GetElementVisual(ClockIcon).StartAnimation("Opacity", fadeDetails);
            ElementCompositionPreview.GetElementVisual(TimeMovingFormattedTextBlockHeader).StartAnimation("Opacity", fadeDetails);
            ElementCompositionPreview.GetElementVisual(DistanceFormattedTextBlockHeader).StartAnimation("Opacity", fadeDetails);
            #endregion

            #region HeaderOpaque
            ExpressionAnimation crossfade = compositor.CreateExpressionAnimation();
            crossfade.SetReferenceParameter("scroller", scrollerManipProps);
            
            //80 is number of pixels scrolled by which the element will be at 0 opacity 
            crossfade.Expression = "(1-(Clamp(scroller.Translation.Y*.5, -80, 0)/-80))";  
            BlurPanel.VisualProperties.StartAnimation("FadeValue", crossfade);
            #endregion

            #region MoveTitle
            ExpressionAnimation moveTitle = compositor.CreateExpressionAnimation();
            moveTitle.SetReferenceParameter("scroller", scrollerManipProps);

            //100 is number of pixels scrolled by which the element will be at 0 opacity 
            moveTitle.Expression = "Clamp(scroller.Translation.Y*0.2, -20, 0)";  
            Visual activityName = ElementCompositionPreview.GetElementVisual(ActivityName);
            activityName.StartAnimation("Offset.Y", moveTitle);
            #endregion

            #region Header Colapse
            m_headerColapseExpression = compositor.CreateExpressionAnimation();
            m_headerColapseExpression.SetReferenceParameter("scroller", scrollerManipProps);
            m_headerColapseExpression.SetScalarParameter("minHeight", (float)BlurPanel.ActualHeight - 50); //50 is the desired height of the header when collapsed
            m_headerColapseExpression.Expression = "Clamp(scroller.Translation.Y, -minHeight, 0)";

            m_blurPanelVisual = ElementCompositionPreview.GetElementVisual(BlurPanel);
            m_blurPanelVisual.StartAnimation("Offset.Y", m_headerColapseExpression);

            #endregion
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
            Func<object, bool, Uri> getImageForPhoto = (o, large) => 
            {
                if (large)
                    return new Uri(((Photo)o).ImageLarge);
                else
                    return new Uri(((Photo)o).ImageThumbnail);
            };
            GridView gridView = (GridView)sender;
            GridViewItem item = (GridViewItem)gridView.ContainerFromItem(e.ClickedItem);
            CompositionImage image = VisualTreeHelperExtensions.GetFirstDescendantOfType<CompositionImage>(item);

            ContinuityTransition transition = new ContinuityTransition();
            transition.Initialize(Window.Current.Content, image, null);
            
            //Display the imageViewer as an app modal experience.  Margin determines how much of the app is visible around the edges of the dialog
            ImagePopupViewer.Show((Photo) e.ClickedItem, ActivityPhotosGrid.ItemsSource, getImageForPhoto, new Thickness(100, 50, 50, 50), transition);
        }
    }
}