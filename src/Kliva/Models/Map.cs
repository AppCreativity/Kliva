using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Kliva.Helpers;
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
        private string _googleImageApiUrl;
        public string GoogleImageApiUrl
        {
            get { return _googleImageApiUrl; }
            set { Set(() => GoogleImageApiUrl, ref _googleImageApiUrl, value); }
        }
    }
}
