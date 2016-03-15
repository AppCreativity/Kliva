using GalaSoft.MvvmLight.Threading;
using Kliva.Helpers;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Windows.Devices.Geolocation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace Kliva.Models
{
    /// <summary>
    /// This class contains information about the route of an activity.
    /// </summary>
    public class Map : BaseClass
    {

        public Map()
        {
            PropertyChanged += Map_PropertyChanged;
        }

        private static readonly StorageFolder _storage = ApplicationData.Current.LocalFolder;
        private async void Map_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("GoogleImageApiUrl", StringComparison.OrdinalIgnoreCase))
            {
                if (this.GoogleImageApiUrl == null) return;

                StorageFile destinationFile = null;
                FileRandomAccessStream stream = null;

                try
                {
                    destinationFile = await _storage.GetFileAsync(string.Concat(Id, ".map")).AsTask().ConfigureAwait(false);
                }
                catch (FileNotFoundException exception)
                {
                    //File does not yet exist - so we will download it!
                    //DispatcherHelper.CheckBeginInvokeOnUI(() => this.GoogleMapImage = new BitmapImage() { UriSource = new Uri("ms-appx:///Assets/MapDefault.png", UriKind.Absolute), DecodePixelHeight = 220, DecodePixelWidth = 480 });
                }

                bool destinationFileCreated = true;
                if (destinationFile == null)
                {
                    try
                    {
                        destinationFile = await _storage.CreateFileAsync(string.Concat(this.Id, ".map")).AsTask().ConfigureAwait(false);
                        DownloadOperation downloader = ServiceLocator.Current.GetInstance<BackgroundDownloader>().CreateDownload(new Uri(GoogleImageApiUrl), destinationFile);
                        var mapFile = await downloader.StartAsync();
                        stream = (FileRandomAccessStream)await mapFile.ResultFile.OpenAsync(FileAccessMode.Read);
                    }
                    catch (Exception exception)
                    {
                        destinationFileCreated = false;
                    }
                }
                else
                    stream = (FileRandomAccessStream)await destinationFile.OpenAsync(FileAccessMode.Read);

                if (!destinationFileCreated && destinationFile != null)
                {
                    try
                    {
                        await destinationFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    catch (Exception exception)
                    {
                        //TODO: Glenn - implement
                    }
                }
                else
                {
                    if (stream != null)
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            var bitmap = new BitmapImage();

                            //memory storage size
                            //bitmap.DecodePixelHeight = 220;
                            bitmap.DecodePixelHeight = 190;
                            bitmap.DecodePixelWidth = 480;

                            bitmap.SetSource(stream);
                            this.GoogleMapImage = bitmap;
                        });
                }
            }
        }

        /// <summary>
        /// The map id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The map's polyline. This polyline can be converted to coordinates.
        /// </summary>
        [JsonProperty("polyline")]
        public string Polyline { get; set; }

        /// <summary>
        /// A summary of the polyline.
        /// </summary>
        [JsonProperty("summary_polyline")]
        public string SummaryPolyline { get; set; }

        /// <summary>
        /// The resource state gives information about the level of details of the map.
        /// </summary>
        [JsonProperty("resource_state")]
        public int ResourceState { get; set; }

        private List<BasicGeoposition> _geopositions;
        public List<BasicGeoposition> GeoPositions
        {
            get
            {
                return _geopositions ?? (string.IsNullOrEmpty(this.SummaryPolyline) ? (_geopositions = PolylineConverter.DecodePolylinePoints(this.Polyline)) : (_geopositions = PolylineConverter.DecodePolylinePoints(this.SummaryPolyline)));
            }
        }

        /// <summary>
        /// A url to the google api
        /// </summary>
        private string _googleImageApiUrl;
        public string GoogleImageApiUrl
        {
            get { return _googleImageApiUrl; }
            set
            {
                if (value == _googleImageApiUrl)
                    return;

                _googleImageApiUrl = value;
                RaisePropertyChanged();
            }
        }

        private BitmapImage _googleBitmapImage;
        public BitmapImage GoogleMapImage
        {
            get { return _googleBitmapImage; }
            set
            {
                if (value == _googleBitmapImage)
                    return;

                _googleBitmapImage = value;
                RaisePropertyChanged();
            }
        }
    }
}
