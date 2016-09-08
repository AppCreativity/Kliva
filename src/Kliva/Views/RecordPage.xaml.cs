using System;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using Cimbalino.Toolkit.Extensions;
using Kliva.Controls;
using Kliva.Extensions;
using Kliva.ViewModels;

namespace Kliva.Views
{
    public sealed partial class RecordPage : Page
    {
        private readonly MapIcon _currentLocation = new MapIcon();

        private RecordViewModel ViewModel => DataContext as RecordViewModel;

        public RecordPage()
        {
            this.InitializeComponent();
            //TODO: Glenn - Get a bigger icon?
            _currentLocation.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/CurrentPosition.png"));
            _currentLocation.NormalizedAnchorPoint = new Point(0.5, 0.5);
            _currentLocation.ZIndex = 0;

            ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Cleanup();
            base.OnNavigatedFrom(e);
        }

        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(ViewModel.CurrentLocation), StringComparison.OrdinalIgnoreCase))
            {
                TrackingMap.ClearMap<MapIcon>();

                _currentLocation.Location = TrackingMap.Center = ViewModel.CurrentLocation;
                TrackingMap.ZoomLevel = 16.0;
                TrackingMap.MapElements.Add(_currentLocation);
            }
        }

        /// <summary>
        /// Set menu flyout width equal to the container it's in
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnActivityFlyoutOpened(object sender, object e)
        {
            MenuFlyout menuFlyout = sender as MenuFlyout;
            Style style = new Style { TargetType = typeof(MenuFlyoutPresenter) };
            style.Setters.Add(new Setter(MinWidthProperty, ContentPanel.ActualWidth));

            //TODO: Glenn - Can't we just grab : {ThemeResource SplitViewCompactPaneThemeLength} - we can but than it could be different from the actual size during runtime
            //TODO: Convert.ToInt32((Application.Current.Resources["SplitViewCompactPaneThemeLength"] as double?).Value)
            SplitView splitView = ((KlivaApplicationFrame)this.Parent).GetVisualDescendents<SplitView>().FirstOrDefault();
            if (splitView.DisplayMode != SplitViewDisplayMode.Inline)
                style.Setters.Add(new Setter(MarginProperty, new Thickness(splitView.CompactPaneLength, 0, 0, 0)));

            menuFlyout.MenuFlyoutPresenterStyle = style;
        }
    }
}
