using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight.Threading;
using Kliva.Helpers;
using Kliva.Services;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;

namespace Kliva.Models
{
    /// <summary>
    /// This class contains information about the route of an activity.
    /// </summary>
    public partial class Map : BaseClass
    {
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
    }

    /// <summary>
    /// Separated added fields from original response class!
    /// </summary>
    public partial class Map : BaseClass
    {
        public Map()
        {
            PropertyChanged += OnMapPropertyChanged;
        }

        private async void OnMapPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("GoogleImageApiUrl", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(GoogleImageApiUrl))
            {
                string fileName = string.Concat(Id, ".map");
                FileRandomAccessStream fileStream = await ServiceLocator.Current.GetInstance<IOService>().GetFileAsync(fileName);

                if (fileStream == null)
                {
                    if (!string.IsNullOrEmpty(GoogleImageApiUrl))
                    {
                        //TODO: Glenn - reuse HttpClient!
                        HttpClient client = new HttpClient();
                        try
                        {
                            var result = await Task.Run(() => client.GetByteArrayAsync(GoogleImageApiUrl));
                            await ServiceLocator.Current.GetInstance<IOService>().SaveFileAsync(fileName, result);
                            var stream = new InMemoryRandomAccessStream();
                            await stream.WriteAsync(result.AsBuffer());
                            stream.Seek(0);

                            DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                            {
                                var bitmap = new BitmapImage();
                                bitmap.DecodePixelHeight = 190;
                                bitmap.DecodePixelWidth = 480;
                                await bitmap.SetSourceAsync(stream);
                                GoogleImage = bitmap;
                            });
                        }
                        catch (Exception exception)
                        {
                            //TODO: Show exception
                        }
                    }
                }
                else
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                    {
                        var bitmap = new BitmapImage();
                        bitmap.DecodePixelHeight = 190;
                        bitmap.DecodePixelWidth = 480;
                        await bitmap.SetSourceAsync(fileStream);
                        GoogleImage = bitmap;
                    });
                }
            }
        }

        private string _googleImageApiUrl;
        public string GoogleImageApiUrl
        {
            get { return _googleImageApiUrl; }
            set { Set(() => GoogleImageApiUrl, ref _googleImageApiUrl, value); }
        }

        private BitmapImage _googleImage;
        public BitmapImage GoogleImage
        {
            get { return _googleImage; }
            set { Set(() => GoogleImage, ref _googleImage, value); }
        }
    }
}