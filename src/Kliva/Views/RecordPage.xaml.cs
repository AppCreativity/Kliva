using System;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
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
    }
}
