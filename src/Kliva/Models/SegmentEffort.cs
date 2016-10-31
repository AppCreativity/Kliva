using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Kliva.Models.Interfaces;

namespace Kliva.Models
{
    /// <summary>
    /// A segment effort represents an athlete’s attempt at a segment. It can also be 
    /// thought of as a portion of a ride that covers a segment. The object is returned in 
    /// summary or detailed representations. They are currently the same.
    /// </summary>
    public partial class SegmentEffort
    {
        /// <summary>
        /// The segment effort's id.
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// The average cadence.
        /// </summary>
        [JsonProperty("average_cadence")]
        public float AverageCadence { get; set; }

        /// <summary>
        /// The average power in watts.
        /// </summary>
        [JsonProperty("average_watts")]
        public float AveragePower { get; set; }

        /// <summary>
        /// The average heartrate.
        /// </summary>
        [JsonProperty("average_heartrate")]
        public float AverageHeartrate { get; set; }

        /// <summary>
        /// The max heartrate.
        /// </summary>
        [JsonProperty("max_heartrate")]
        public float MaxHeartrate { get; set; }

        /// <summary>
        /// Indicates level of detail
        /// </summary>
        [JsonProperty("resource_state")]
        public int ResourceState { get; set; }

        /// <summary>
        /// The name of the segment.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Detailed segment object.
        /// </summary>
        [JsonProperty("segment")]
        public Segment Segment { get; set; }

        /// <summary>
        /// Meta information about the activity.
        /// </summary>
        [JsonProperty("activity")]
        public ActivityMeta Activity { get; set; }

        /// <summary>
        /// Meta information about the athlete.
        /// </summary>
        [JsonProperty("athlete")]
        public AthleteMeta Athlete { get; set; }

        /// <summary>
        /// 1-10 rank on segment at time of upload
        /// </summary>
        [JsonProperty("kom_rank")]
        public int? KingOfMountainRank { get; set; }

        /// <summary>
        /// 1-3 personal record on segment at time of upload
        /// </summary>
        [JsonProperty("pr_rank")]
        public int? PersonalRecordRank { get; set; }

        /// <summary>
        /// Moving time in seconds.
        /// </summary>
        [JsonProperty("moving_time")]
        public int MovingTime { get; set; }

        /// <summary>
        /// Elapsed time in seconds.
        /// </summary>
        [JsonProperty("elapsed_time")]
        public int ElapsedTime { get; set; }

        /// <summary>
        /// Start date and time.
        /// </summary>
        [JsonProperty("start_date")]
        public string StartDate { get; set; }

        /// <summary>
        /// Returns the StartDate-Property as a DateTime object.
        /// </summary>
        public DateTime DateTimeStart
        {
            get { return DateTime.Parse(StartDate); }
        }

        /// <summary>
        /// Returns the moving time as a TimeSpan object rather than an int value.
        /// </summary>
        public TimeSpan MovingTimeSpan
        {
            get { return TimeSpan.FromSeconds(MovingTime); }
        }

        /// <summary>
        /// Returns the elapsed time as a TimeSpan object rather than an int value.
        /// </summary>
        public TimeSpan ElapsedTimeSpan
        {
            get { return TimeSpan.FromSeconds(ElapsedTime); }
        }

        /// <summary>
        /// Distance in meters.
        /// </summary>
        [JsonProperty("distance")]
        public float Distance { get; set; }

        /// <summary>
        /// The activity stream index of the start of this effort
        /// </summary>
        [JsonProperty("start_index")]
        public int StartIndex { get; set; }

        /// <summary>
        /// The activity stream index of the end of this effort
        /// </summary>
        [JsonProperty("end_index")]
        public int EndIndex { get; set; }
    }

    /// <summary>
    /// Separated added fields from original response class!
    /// </summary>
    public partial class SegmentEffort : BaseClass, ISupportUserMeasurementUnits
    {
        /// <summary>
        /// Take Segment distance to allign result with actual Strava website!
        /// </summary>
        public float AverageSpeed => Segment.Distance/ElapsedTime;

        private ObservableCollection<StatisticsGroup> _statistics = new ObservableCollection<StatisticsGroup>();
        public ObservableCollection<StatisticsGroup> Statistics
        {
            get { return _statistics; }
            set { Set(() => Statistics, ref _statistics, value); }
        }

        public DistanceUnitType MeasurementUnit
        {
            get;
            private set;
        }

        public void SetUserMeasurementUnits(DistanceUnitType measurementUnit)
        {
            MeasurementUnit = measurementUnit;

            DistanceUserMeasurementUnit = new UserMeasurementUnitMetric(Distance, DistanceUnitType.Metres, MeasurementUnit);
            AverageSpeedMeasurementUnit = new UserMeasurementUnitMetric(AverageSpeed, SpeedUnit.MetresPerSecond, SpeedUnit.MetresPerSecond);
        }

        public UserMeasurementUnitMetric DistanceUserMeasurementUnit
        {
            get;
            private set;
        }

        public UserMeasurementUnitMetric AverageSpeedMeasurementUnit
        {
            get;
            private set;
        }
    }
}
