using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Kliva.ViewModels.Interfaces;
using Windows.UI.Xaml.Controls;
using Cimbalino.Toolkit.Extensions;
using Kliva.Helpers;
using Kliva.Models;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Composition;
using System;
using Windows.UI.Xaml.Hosting;
using System.Numerics;
using Kliva.ViewModels;

namespace Kliva.Controls
{
    public sealed partial class ActivityFeedControl : UserControl
    {
        private ScrollViewer _scrollViewer;
        private bool _isAtTop = true;


        #region Compositor Member vars
        private Compositor _compositor;
        private const float ENTRANCE_ANIMATION_DURATION = 350;
        private const float ENTRANCE_ANIMATION_OPACITY_STAGGER_DELAY = 25;
        private const float ENTRANCE_ANIMATION_TEXT_STAGGER_DELAY = 25;
        #endregion


        //private IStravaViewModel ViewModel => DataContext as IStravaViewModel;
        private MainViewModel ViewModel => DataContext as MainViewModel;

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
            _scrollViewer = ActivityList.GetVisualDescendents<ScrollViewer>().FirstOrDefault();
            if (_scrollViewer != null)
            {
                _scrollViewer.ViewChanged += OnScrollViewerViewChanged;
                _scrollViewer.ViewChanging += OnScrollViewerViewChanging;
                _scrollViewer.IsScrollInertiaEnabled = true;
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

        private bool IsDesktopState()
        {
            return AdaptiveStates.CurrentState.Name == Desktop.Name;
        }

        private void ActivityList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.ActivityInvoked((ActivitySummary)e.ClickedItem);
        }

        // To hook up per-item parallax and staggering animations we need to hook the render pipeline of the list
        private void ActivityList_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            // TODO: Set up per-item parallax

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