using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Kliva.ViewModels.Interfaces;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Cimbalino.Toolkit.Extensions;
using Kliva.Helpers;
using Kliva.Models;
using Kliva.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace Kliva.Controls
{
    public sealed partial class ActivityFeedControl : UserControl
    {
        private ScrollViewer _scrollViewer;
        private bool _isAtTop = true;

        private Compositor _compositor;
        private Compositor Compositor
        {
            get { return _compositor ?? (_compositor = ElementCompositionPreview.GetElementVisual(this).Compositor); }
        }

        #region PullToRefresh Member Variables
        private const float REFRESH_ICON_MAX_OFFSET_Y = 36.0f;

        private CompositionPropertySet _scrollerViewerManipulation;
        private ExpressionAnimation _rotationAnimation, _opacityAnimation, _offsetAnimation;
        private ScalarKeyFrameAnimation _resetAnimation, _loadingAnimation;
        private Visual _borderVisual;
        private Visual _refreshIconVisual;
        private float _refreshIconOffsetY;
        
        private bool _refresh;
        private DateTime _pulledDownTime, _restoredTime;
        #endregion

        private IStravaViewModel ViewModel => DataContext as IStravaViewModel;

        public ActivityFeedControl()
        {
            this.InitializeComponent();

            DataContextChanged += (sender, args) => this.Bindings.Update();
        }

        private void OnActivityListLoaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = ActivityList.GetVisualDescendents<ScrollViewer>().FirstOrDefault();
            if (_scrollViewer != null)
            {
                _scrollViewer.ViewChanged += OnScrollViewerViewChanged;
                _scrollViewer.ViewChanging += OnScrollViewerViewChanging;
                _scrollViewer.IsScrollInertiaEnabled = true;

                _scrollViewer.DirectManipulationStarted += OnDirectManipStarted;
                _scrollViewer.DirectManipulationCompleted += OnDirectManipCompleted;

                #region PullToRefresh Animation Setup
                // Retrieve the ScrollViewer manipulation and the Compositor.
                _scrollerViewerManipulation = ElementCompositionPreview.GetScrollViewerManipulationPropertySet(_scrollViewer);
                //Compositor = _scrollerViewerManipulation.Compositor;

                // Create a rotation expression animation based on the overpan distance of the ScrollViewer.
                _rotationAnimation = Compositor.CreateExpressionAnimation();
                _rotationAnimation.SetScalarParameter("MyMultiplier", 10.0f);
                _rotationAnimation.SetScalarParameter("MyMaxDegree", 400.0f);
                _rotationAnimation.SetReferenceParameter("MyScrollManipulation", _scrollerViewerManipulation);
                _rotationAnimation.Expression = "min(max(0, MyScrollManipulation.Translation.Y) * MyMultiplier, MyMaxDegree)";

                // Create an opacity expression animation based on the overpan distance of the ScrollViewer.
                _opacityAnimation = Compositor.CreateExpressionAnimation();
                _opacityAnimation.SetScalarParameter("MyDivider", 30.0f);
                _opacityAnimation.SetReferenceParameter("MyScrollManipulation", _scrollerViewerManipulation);
                _opacityAnimation.Expression = "min(max(0, MyScrollManipulation.Translation.Y) / MyDivider, 1)";

                // Create an offset expression animation based on the overpan distance of the ScrollViewer.
                _offsetAnimation = Compositor.CreateExpressionAnimation();
                _offsetAnimation.SetScalarParameter("MyDivider", 30.0f);
                _offsetAnimation.SetScalarParameter("MyMaxOffsetY", REFRESH_ICON_MAX_OFFSET_Y);
                _offsetAnimation.SetReferenceParameter("MyScrollManipulation", _scrollerViewerManipulation);
                _offsetAnimation.Expression = "(min(max(0, MyScrollManipulation.Translation.Y) / MyDivider, 1)) * MyMaxOffsetY";

                // Create a keyframe animation to reset properties like Offset.Y, Opacity, etc.
                _resetAnimation = Compositor.CreateScalarKeyFrameAnimation();
                _resetAnimation.InsertKeyFrame(1.0f, 0.0f);

                // Create a loading keyframe animation (in this case, a rotation animation). 
                _loadingAnimation = Compositor.CreateScalarKeyFrameAnimation();
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

                _refreshIconVisual.StartAnimation(nameof(_refreshIconVisual.RotationAngleInDegrees), _rotationAnimation);
                _refreshIconVisual.StartAnimation(nameof(_refreshIconVisual.Opacity), _opacityAnimation);
                _refreshIconVisual.StartAnimation($"{nameof(_refreshIconVisual.Offset)}.{nameof(_refreshIconVisual.Offset.Y)}", _offsetAnimation);
                _borderVisual.StartAnimation($"{nameof(_borderVisual.Offset)}.{nameof(_borderVisual.Offset.Y)}", _offsetAnimation);
                #endregion
            }
        }

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

        private async void OnDirectManipCompleted(object sender, object e)
        {
            Windows.UI.Xaml.Media.CompositionTarget.Rendering -= OnCompositionTargetRendering;

            var cancelled = (_restoredTime - _pulledDownTime) > TimeSpan.FromMilliseconds(250);

            if (_refresh)
            {
                if (cancelled)
                    StartResetAnimations();
                else
                    await StartLoadingAnimation(StartResetAnimations);
            }
        }

        private void OnDirectManipStarted(object sender, object e)
        {
            Windows.UI.Xaml.Media.CompositionTarget.Rendering += OnCompositionTargetRendering;

            _refresh = false;
        }

        private async Task StartLoadingAnimation(Action completed)
        {
            var modes = _scrollViewer.ManipulationMode;

            // Create a short delay to allow the expression rotation animation to more smoothly transition
            // to the new keyframe animation
            await Task.Delay(100);

            _refreshIconVisual.StartAnimation(nameof(_refreshIconVisual.RotationAngleInDegrees), _loadingAnimation);

            _scrollViewer.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.None;

            //TODO: Glenn - reload data!
            //((MainViewModel)ViewModel).ActivityIncrementalCollection.LoadNewData();

            // Gratiuitous demo delay to ensure the animation shows :-)
            await Task.Delay(1500);

            completed();

            _scrollViewer.ManipulationMode = modes;
        }

        private void StartResetAnimations()
        {
            var batch = Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            batch.Completed += (s, e) =>
            {
                _refreshIconVisual.StartAnimation(nameof(_refreshIconVisual.RotationAngleInDegrees), _rotationAnimation);
                _refreshIconVisual.StartAnimation(nameof(_refreshIconVisual.Opacity), _opacityAnimation);
                _refreshIconVisual.StartAnimation($"{nameof(_refreshIconVisual.Offset)}.{nameof(_refreshIconVisual.Offset.Y)}", _offsetAnimation);
                _borderVisual.StartAnimation($"{nameof(_borderVisual.Offset)}.{nameof(_borderVisual.Offset.Y)}", _offsetAnimation);
            };

            _borderVisual.StartAnimation($"{nameof(_borderVisual.Offset)}.{nameof(_borderVisual.Offset.Y)}", _resetAnimation);
            _refreshIconVisual.StartAnimation(nameof(_refreshIconVisual.Opacity), _resetAnimation);

            batch.End();
        }

        private void OnCompositionTargetRendering(object sender, object e)
        {
            _refreshIconVisual.StopAnimation($"{nameof(_refreshIconVisual.Offset)}.{nameof(_refreshIconVisual.Offset.Y)}");

            _refreshIconOffsetY = _refreshIconVisual.Offset.Y;
            if (!_refresh)
            {
                _refresh = _refreshIconOffsetY == REFRESH_ICON_MAX_OFFSET_Y;
            }

            if (_refreshIconOffsetY == REFRESH_ICON_MAX_OFFSET_Y)
            {
                _pulledDownTime = DateTime.Now;

                // Stop the Opacity animation on the RefreshIcon and the Offset.Y animation on the Border (ScrollViewer's host)
                _refreshIconVisual.StopAnimation(nameof(_refreshIconVisual.Opacity));
                _borderVisual.StopAnimation($"{nameof(_borderVisual.Offset)}.{nameof(_borderVisual.Offset.Y)}");
            }

            if (_refresh && _refreshIconOffsetY <= 1)
            {
                _restoredTime = DateTime.Now;
            }

            _refreshIconVisual.StartAnimation($"{nameof(_refreshIconVisual.Offset)}.{nameof(_refreshIconVisual.Offset.Y)}", _offsetAnimation);
        }

        private void OnGoToTopButtonClick(object sender, RoutedEventArgs e)
        {
            ScrollToTop();
        }

        private void ScrollToTop()
        {
            _scrollViewer?.ScrollToVerticalOffsetWithAnimation(0);
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

        private bool IsDesktopState()
        {
            return AdaptiveStates.CurrentState.Name == Desktop.Name;
        }

        private void ActivityList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.ActivityInvoked((ActivitySummary)e.ClickedItem);
        }
    }
}
