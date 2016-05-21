using System.Linq;
using Windows.UI.Xaml;
using Kliva.ViewModels.Interfaces;
using Windows.UI.Xaml.Controls;
using Cimbalino.Toolkit.Extensions;
using Kliva.Helpers;
using Kliva.Models;

namespace Kliva.Controls
{
    public sealed partial class ActivityFeedControl : UserControl
    {
        private ScrollViewer _scrollViewer;
        private bool _isAtTop = true;

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
