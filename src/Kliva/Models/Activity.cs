using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Kliva.Models
{
    /// <summary>
    /// Activities are the base object for Strava runs, rides, swims etc.
    /// </summary>
    public partial class Activity : ActivitySummary
    {
        /// <summary>
        /// A list of segment effort objects.
        /// </summary>
        [JsonProperty("segment_efforts")]
        public List<SegmentEffort> SegmentEfforts { get; set; }

        /// <summary>
        /// A summary of the gear used.
        /// </summary>
        //[JsonProperty("gear")]
        //public GearSummary Gear { get; set; }

        /// <summary>
        /// the burned kilocalories.
        /// </summary>
        [JsonProperty("calories")]
        public float Calories { get; set; }

        /// <summary>
        /// The activity's description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }

    /// <summary>
    /// Separated added fields from original response class!
    /// </summary>
    public partial class Activity
    {
        public int SegmentEffortsCount => SegmentEfforts?.Count ?? 0;

        private ObservableCollection<StatisticsDetail> _statistics = new ObservableCollection<StatisticsDetail>();
        public ObservableCollection<StatisticsDetail> Statistics
        {
            get { return _statistics; }
            set { Set(() => Statistics, ref _statistics, value); }
        }
    }
}
