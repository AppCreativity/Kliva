using Kliva.Helpers;
using Kliva.Models;
using Kliva.ViewModels;
using Kliva.ViewModels.Interfaces;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace Kliva.Controls
{
    public sealed partial class ActivityFeedControl : UserControl
    {
        private ScrollViewer _scrollViewer;
        private bool _isAtTop = true;

        #region Compositor Member vars
        private Compositor _compositor;

        #region Stagger constants
        private const float ENTRANCE_ANIMATION_DURATION = 350;
        private const float ENTRANCE_ANIMATION_OPACITY_STAGGER_DELAY = 25;
        private const float ENTRANCE_ANIMATION_TEXT_STAGGER_DELAY = 25;
        #endregion

        #region PTR Member Variables
        private CompositionPropertySet _scrollerViewerManipulation;
        private ExpressionAnimation _rotationAnimation, _opacityAnimation, _offsetAnimation;
        private ScalarKeyFrameAnimation _resetAnimation, _loadingAnimation;
        private Visual _borderVisual;
        private Visual _refreshIconVisual;
        private float _refreshIconOffsetY;
        private const float REFRESH_ICON_MAX_OFFSET_Y = 36.0f;
        bool _refresh;
        private DateTime _pulledDownTime, _restoredTime;
        #endregion

        #endregion

        private IStravaViewModel ViewModel => DataContext as IStravaViewModel;

        public ActivityFeedControl()
        {
            this.InitializeComponent();

            InitializeCompositor();

            DataContextChanged += (sender, args) => this.Bindings.Update();
        }

        private void InitializeCompositor()
        {
            // get the compositor
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        }

        private void OnActivityListLoaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = ActivityList.GetScrollViewer();
            if (_scrollViewer != null)
            {
                _scrollViewer.ViewChanged += OnScrollViewerViewChanged;
                _scrollViewer.ViewChanging += OnScrollViewerViewChanging;
                _scrollViewer.IsScrollInertiaEnabled = true;
                _scrollViewer.DirectManipulationStarted += OnDirectManipStarted;
                _scrollViewer.DirectManipulationCompleted += OnDirectManipCompleted;

                #region PTR Animation Setup
                // Retrieve the ScrollViewer manipulation and the Compositor.
                _scrollerViewerManipulation = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollViewer);
                _compositor = _scrollerViewerManipulation.Compositor;

                // Create a rotation expression animation based on the overpan distance of the ScrollViewer.
                _rotationAnimation = _compositor.CreateExpressionAnimation();
                _rotationAnimation.SetScalarParameter("Multiplier", 10.0f);
                _rotationAnimation.SetScalarParameter("MaxDegree", 400.0f);
                _rotationAnimation.SetReferenceParameter("ScrollManipulation", _scrollerViewerManipulation);
                _rotationAnimation.Expression = "min(max(0, ScrollManipulation.Translation.Y) * Multiplier, MaxDegree)";

                // Create an opacity expression animation based on the overpan distance of the ScrollViewer.
                _opacityAnimation = _compositor.CreateExpressionAnimation();
                _opacityAnimation.SetScalarParameter("Divider", 30.0f);
                _opacityAnimation.SetReferenceParameter("ScrollManipulation", _scrollerViewerManipulation);
                _opacityAnimation.Expression = "min(max(0, ScrollManipulation.Translation.Y) / Divider, 1)";

                // Create an offset expression animation based on the overpan distance of the ScrollViewer.
                _offsetAnimation = _compositor.CreateExpressionAnimation();
                _offsetAnimation.SetScalarParameter("Divider", 30.0f);
                _offsetAnimation.SetScalarParameter("MaxOffsetY", REFRESH_ICON_MAX_OFFSET_Y);
                _offsetAnimation.SetReferenceParameter("ScrollManipulation", _scrollerViewerManipulation);
                _offsetAnimation.Expression = "(min(max(0, ScrollManipulation.Translation.Y) / Divider, 1)) * MaxOffsetY";

                // Create a keyframe animation to reset properties like Offset.Y, Opacity, etc.
                _resetAnimation = _compositor.CreateScalarKeyFrameAnimation();
                _resetAnimation.InsertKeyFrame(1.0f, 0.0f);

                // Create a loading keyframe animation (in this case, a rotation animation). 
                _loadingAnimation = _compositor.CreateScalarKeyFrameAnimation();
                _loadingAnimation.InsertKeyFrame(1.0f, 360);
                _loadingAnimation.Duration = TimeSpan.FromMilliseconds(800);
                _loadingAnimation.IterationBehavior = AnimationIterationBehavior.Forever;

                // Get the RefreshIcon's Visual.
                _refreshIconVisual = ElementCompositionPreview.GetElementVisual(RefreshIcon);
                // Set the center point for the rotation animation.
                _refreshIconVisual.CenterPoint = new Vector3(Convert.ToSingle(RefreshIcon.ActualWidth / 2), Convert.ToSingle(RefreshIcon.ActualHeight / 2), 0);

                // Get the ListView's inner Border's Visual.
                var border = (Border)VisualTreeHelper.GetChild(ActivityList, 0);
                _borderVisual = ElementCompositionPreview.GetElementVisual(border);

                _refreshIconVisual.StartAnimation("RotationAngleInDegrees", _rotationAnimation);
                _refreshIconVisual.StartAnimation("Opacity", _opacityAnimation);
                _refreshIconVisual.StartAnimation("Offset.Y", _offsetAnimation);
                _borderVisual.StartAnimation("Offset.Y", _offsetAnimation);
                #endregion
            }
        }

        #region Supporting PTR methods
        async private void OnDirectManipCompleted(object sender, object e)
        {
            Windows.UI.Xaml.Media.CompositionTarget.Rendering -= OnCompositionTargetRendering;

            var cancelled = (_restoredTime - _pulledDownTime) > TimeSpan.FromMilliseconds(250);

            if (_refresh)
            {
                if (cancelled)
                {
                    StartResetAnimations();
                }
                else
                {
                    await StartLoadingAnimation(() => StartResetAnimations());
                }
            }
        }

        private void OnDirectManipStarted(object sender, object e)
        {
            Windows.UI.Xaml.Media.CompositionTarget.Rendering += OnCompositionTargetRendering;

            _refresh = false;
        }

        async Task StartLoadingAnimation(Action completed)
        {
            var modes = _scrollViewer.ManipulationMode;

            // Create a short delay to allow the expression rotation animation to more smoothly transition
            // to the new keyframe animation
            await Task.Delay(100);

            _refreshIconVisual.StartAnimation("RotationAngleInDegrees", _loadingAnimation);

            _scrollViewer.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.None;

            ((MainViewModel)ViewModel).ActivityIncrementalCollection.LoadNewData();

            // Gratiuitous demo delay to ensure the animation shows :-)
            await Task.Delay(1500);

            completed();

            _scrollViewer.ManipulationMode = modes;
        }

        void StartResetAnimations()
        {
            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            batch.Completed += (s, e) =>
            {
                _refreshIconVisual.StartAnimation("RotationAngleInDegrees", _rotationAnimation);
                _refreshIconVisual.StartAnimation("Opacity", _opacityAnimation);
                _refreshIconVisual.StartAnimation("Offset.Y", _offsetAnimation);
                _borderVisual.StartAnimation("Offset.Y", _offsetAnimation);
            };

            _borderVisual.StartAnimation("Offset.Y", _resetAnimation);
            _refreshIconVisual.StartAnimation("Opacity", _resetAnimation);

            batch.End();
        }

        private void OnCompositionTargetRendering(object sender, object e)
        {
            _refreshIconVisual.StopAnimation("Offset.Y");

            _refreshIconOffsetY = _refreshIconVisual.Offset.Y;
            if (!_refresh)
            {
                _refresh = _refreshIconOffsetY == REFRESH_ICON_MAX_OFFSET_Y;
            }

            if (_refreshIconOffsetY == REFRESH_ICON_MAX_OFFSET_Y)
            {
                _pulledDownTime = DateTime.Now;

                // Stop the Opacity animation on the RefreshIcon and the Offset.Y animation on the Border (ScrollViewer's host)
                _refreshIconVisual.StopAnimation("Opacity");
                _borderVisual.StopAnimation("Offset.Y");
            }

            if (_refresh && _refreshIconOffsetY <= 1)
            {
                _restoredTime = DateTime.Now;
            }

            _refreshIconVisual.StartAnimation("Offset.Y", _offsetAnimation);
        } 
        #endregion

        private void OnScrollViewerViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (_scrollViewer == null)
                return;

            if (e.IsInertial)
            {
                if (!_isAtTop)
                {
                    VisualStateManager.GoToState(this, "ShowGoToButton", true);
                }
            }
        }

        private void OnScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (_scrollViewer == null)
                return;

            _isAtTop = _scrollViewer.VerticalOffset < 100;

            if (_isAtTop)
            {
                VisualStateManager.GoToState(this, "HideGoToButton", true);
            }
        }

        private void OnGoToTopButtonClick(object sender, RoutedEventArgs e)
        {
            ScrollToTop();
        }

        private void ScrollToTop()
        {
            _scrollViewer?.ScrollToVerticalOffsetWithAnimation(0, 0.5);
        }

        private void ActivityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsDesktopState())
            {
                // Only update the ViewModel if we are in the wide state where the list supports
                // selection
                ViewModel.SelectedActivity = (ActivitySummary)e.AddedItems.FirstOrDefault();
            }
        }

        private void AdaptiveStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (IsDesktopState())
            {
                // Make sure the selected item is properly restored if we switch to the wide state
                ActivityList.SelectionMode = ListViewSelectionMode.Single;
                ActivityList.SelectedItem = ViewModel.SelectedActivity;
            }
        }

        private void OnActivityListUnloaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer.DirectManipulationStarted -= OnDirectManipStarted;
            _scrollViewer.DirectManipulationCompleted -= OnDirectManipCompleted;
        }

        private bool IsDesktopState()
        {
            return AdaptiveStates.CurrentState.Name == Desktop.Name;
        }

        private void ActivityList_ItemClick(object sender, ItemClickEventArgs e)
        {
            #region Enable Connected Animation for Athlete Profile Pic
            // Get the profile pic element from the source
            ListViewItem lvi = ActivityList.ContainerFromItem(e.ClickedItem) as ListViewItem;
            UserControl root = lvi.ContentTemplateRoot as UserControl;
            Ellipse profileImage = root.FindName("AthleteProfilePicture") as Ellipse;

            // Verify we can set up a ConnectedAnimation
            if (ApiInformation.IsTypePresent("Windows.UI.Xaml.Media.Animation.ConnectedAnimationService"))
            {
#if CONNECTANIMATIONS
                ConnectedAnimationService cas = ConnectedAnimationService.GetForCurrentView();
                cas.PrepareToAnimate("AthleteProfilePicture", profileImage);
#endif
            }
            #endregion

            ViewModel.ActivityInvoked((ActivitySummary)e.ClickedItem);
        }

        // To hook up per-item staggering animations we need to hook the render pipeline of the list
        private void ActivityList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            int index = args.ItemIndex;
            var root = args.ItemContainer.ContentTemplateRoot as UserControl;
            var item = args.Item as ActivitySummary;

            // Don't run an entrance animation if we're in recycling
            if (!args.InRecycleQueue)
            {
                args.ItemContainer.Loaded += ItemContainer_Loaded;
            }

            // Collapse the BottomPanel if we're using a recycled container that had it before
            if (args.InRecycleQueue && item.OtherAthleteCount > 0)
            {
                var bottomPanel = root.FindName("BottomPanel") as RelativePanel;
                bottomPanel.Visibility = Visibility.Collapsed;
            }

            // If we have related athletes, then undefer the BottomPanel and show it
            if (args.InRecycleQueue == false && item.OtherAthleteCount > 0)
            {
                // undefer the bottom panel and show it
                var bottomPanel = root.FindName("BottomPanel") as RelativePanel;
                bottomPanel.Visibility = Visibility.Visible;
            }

            args.Handled = true;
        }

        private void ItemContainer_Loaded(object sender, RoutedEventArgs e)
        {
            var itemsPanel = (ItemsStackPanel)ActivityList.ItemsPanelRoot;
            var itemContainer = (ListViewItem)sender;
            var itemIndex = ActivityList.IndexFromContainer(itemContainer);

            var uc = itemContainer.ContentTemplateRoot as UserControl;
            var childPanel = uc.FindName("ActivityListItemPanel") as RelativePanel;

            // Don't animate if we're not in the visible viewport
            if (itemIndex >= itemsPanel.FirstVisibleIndex && itemIndex <= itemsPanel.LastVisibleIndex)
            {
                var itemVisual = ElementCompositionPreview.GetElementVisual(itemContainer);

                float width = (float)childPanel.RenderSize.Width;
                float height = (float)childPanel.RenderSize.Height;
                itemVisual.Size = new Vector2(width, height);
                itemVisual.CenterPoint = new Vector3(width / 2, height / 2, 0f);
                itemVisual.Scale = new Vector3(1, 1, 1); // new Vector3(0.25f, 0.25f, 0);
                itemVisual.Opacity = 0f;
                itemVisual.Offset = new Vector3(0, 100, 0);

                // Create KeyFrameAnimations
                KeyFrameAnimation offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
                offsetAnimation.InsertExpressionKeyFrame(1f, "0");
                offsetAnimation.Duration = TimeSpan.FromMilliseconds(1250);
                offsetAnimation.DelayTime = TimeSpan.FromMilliseconds(itemIndex * 100);

                KeyFrameAnimation scaleAnimation = _compositor.CreateVector3KeyFrameAnimation();
                scaleAnimation.InsertExpressionKeyFrame(0, "Vector3(1, 1, 0)");
                scaleAnimation.InsertExpressionKeyFrame(0.1f, "Vector3(0.05, 0.05, 0)");
                scaleAnimation.InsertExpressionKeyFrame(1f, "Vector3(1, 1, 0)");
                scaleAnimation.Duration = TimeSpan.FromMilliseconds(1000);
                scaleAnimation.DelayTime = TimeSpan.FromMilliseconds(itemIndex * 100);

                KeyFrameAnimation fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
                fadeAnimation.InsertExpressionKeyFrame(1f, "1");
                fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);
                fadeAnimation.DelayTime = TimeSpan.FromMilliseconds(itemIndex * 100);

                // Start animations
                itemVisual.StartAnimation("Offset.Y", offsetAnimation);
                itemVisual.StartAnimation("Scale", scaleAnimation);
                itemVisual.StartAnimation("Opacity", fadeAnimation);
            }

            itemContainer.Loaded -= ItemContainer_Loaded;
        }
    }
}