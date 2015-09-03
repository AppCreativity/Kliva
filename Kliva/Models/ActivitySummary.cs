using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kliva.Models
{
    /// <summary>
    /// Represents a less detailed version of an activity.
    /// </summary>
    public partial class ActivitySummary : ActivityMeta
    {
        /// <summary>
        /// The activity's name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The external id of the activity. Provided upon upload. Can not be changed.
        /// </summary>
        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

        /// <summary>
        /// The type of the activity.
        /// </summary>
        [JsonProperty("type")]
        private string _type { get; set; }

        /// <summary>
        /// The type of the activity.
        /// </summary>
        public ActivityType Type
        {
            get { return (ActivityType)Enum.Parse(typeof(ActivityType), _type); }
        }

        /// <summary>
        /// The distance travelled.
        /// </summary>
        [JsonProperty("distance")]
        public float Distance { get; set; }

        /// <summary>
        /// Time in movement in seconds.
        /// </summary>
        [JsonProperty("moving_time")]
        public int MovingTime { get; set; }

        /// <summary>
        /// The elapsed time in seconds.
        /// </summary>
        [JsonProperty("elapsed_time")]
        public int ElapsedTime { get; set; }

        /// <summary>
        /// The total elevation gain in meters.
        /// </summary>
        [JsonProperty("total_elevation_gain")]
        public float ElevationGain { get; set; }

        /// <summary>
        /// True if the currently authenticated athlete has kudoed this activity.
        /// </summary>
        [JsonProperty("has_kudoed")]
        public bool HasKudoed { get; set; }

        /// <summary>
        /// The athlete's average heartrate during this activity.
        /// </summary>
        [JsonProperty("average_heartrate")]
        public float AverageHeartrate { get; set; }

        /// <summary>
        /// The athlete's max heartrate.
        /// </summary>
        [JsonProperty("max_heartrate")]
        public float MaxHeartrate { get; set; }

        /// <summary>
        /// Only present if activity is owned by authenticated athlete, returns 0 if not truncated by privacy zones.
        /// </summary>
        [JsonProperty("truncated")]
        public int? Truncated { get; set; }

        /// <summary>
        /// The city where this activity was started.
        /// </summary>
        [JsonProperty("city")]
        public string City { get; set; }

        /// <summary>
        /// The state where this activity was started.
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }

        /// <summary>
        /// The country where this activity was started.
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// The id of the gear used.
        /// </summary>
        [JsonProperty("gear_id")]
        public string GearId { get; set; }

        /// <summary>
        /// The average speed in meters per seconds.
        /// </summary>
        [JsonProperty("average_speed")]
        public float AverageSpeed { get; set; }

        /// <summary>
        /// The max speed in meters per second.
        /// </summary>
        [JsonProperty("max_speed")]
        public float MaxSpeed { get; set; }

        /// <summary>
        /// The average cadence. Only returned if activity is a bike ride.
        /// </summary>
        [JsonProperty("average_cadence")]
        public float AverageCadence { get; set; }

        /// <summary>
        /// The average temperature during this activity. Only returned if data is provided upon upload.
        /// </summary>
        [JsonProperty("average_temp")]
        public float AverageTemperature { get; set; }

        /// <summary>
        /// The average power during this activity. Only returned if data is provided upon upload.
        /// </summary>
        [JsonProperty("average_watts")]
        public float AveragePower { get; set; }

        /// <summary>
        /// Kilojoules. Rides only.
        /// </summary>
        [JsonProperty("kilojoules")]
        public float Kilojoules { get; set; }

        /// <summary>
        /// True if the activity was recorded on a stationary trainer.
        /// </summary>
        [JsonProperty("trainer")]
        public bool IsTrainer { get; set; }

        /// <summary>
        /// True if activity is a a commute.
        /// </summary>
        [JsonProperty("commute")]
        public bool IsCommute { get; set; }

        /// <summary>
        /// True if the ride was crated manually.
        /// </summary>
        [JsonProperty("manual")]
        public bool IsManual { get; set; }

        /// <summary>
        /// True if the activity is private.
        /// </summary>
        [JsonProperty("private")]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// True if the activity was flagged.
        /// </summary>
        [JsonProperty("flagged")]
        public bool IsFlagged { get; set; }

        /// <summary>
        /// Achievement count.
        /// </summary>
        [JsonProperty("achievement_count")]
        public int AchievementCount { get; set; }

        /// <summary>
        /// Activity's kudos count.
        /// </summary>
        [JsonProperty("kudos_count")]
        public int KudosCount { get; set; }

        /// <summary>
        /// Activity's comment count.
        /// </summary>
        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        /// <summary>
        /// Number of athletes on this activity.
        /// </summary>
        [JsonProperty("athlete_count")]
        public int AthleteCount { get; set; }

        /// <summary>
        /// Number of photos.
        /// </summary>
        [JsonProperty("photo_count")]
        public int PhotoCount { get; set; }

        /// <summary>
        /// Start date of the activity.
        /// </summary>
        [JsonProperty("start_date")]
        public string StartDate { get; set; }

        /// <summary>
        /// Local start date of the activity.
        /// </summary>
        [JsonProperty("start_date_local")]
        public string StartDateLocal { get; set; }

        /// <summary>
        /// Returns the StartDate-Property as a DateTime object.
        /// </summary>
        public DateTime DateTimeStart
        {
            get { return DateTime.Parse(StartDate); }
        }

        /// <summary>
        /// Returns the StartDateLocal-Property as a DateTime object.
        /// </summary>
        public DateTime DateTimeStartLocal
        {
            get { return DateTime.Parse(StartDateLocal); }
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
        /// Timezone of the activity.
        /// </summary>
        [JsonProperty("timezone")]
        public string TimeZone { get; set; }

        /// <summary>
        /// Coordinate where the activity was started.
        /// </summary>
        [JsonProperty("start_latlng")]
        public List<double> StartPoint { get; set; }

        /// <summary>
        /// Coordinate where the activity was started.
        /// </summary>
        public double? StartLatitude
        {
            get
            {
                if (StartPoint != null && StartPoint.Count > 0)
                    return StartPoint.ElementAt(0);

                return null;
            }
        }

        /// <summary>
        /// Rides with power meter data only similar to xPower or Normalized Power.
        /// </summary>
        [JsonProperty("weighted_average_watts")]
        public int WeightedAverageWatts { get; set; }

        /// <summary>
        /// Coordinate where the activity was started.
        /// </summary>
        public double? StartLongitude
        {
            get
            {
                if (StartPoint != null && StartPoint.Count > 0)
                    return StartPoint.ElementAt(1);

                return null;
            }
        }

        /// <summary>
        /// Coordinate where the activity was ended.
        /// </summary>
        [JsonProperty("end_latlng")]
        public List<double> EndPoint { get; set; }

        /// <summary>
        /// Coordinate where the activity was ended.
        /// </summary>
        public double? EndLatitude
        {
            get
            {
                if (EndPoint != null && EndPoint.Count > 0)
                    return EndPoint.ElementAt(0);

                return null;
            }
        }

        /// <summary>
        /// Coordinate where the activity was ended.
        /// </summary>
        public double? EndLongitude
        {
            get
            {
                if (EndPoint != null && EndPoint.Count > 0)
                    return EndPoint.ElementAt(1);

                return null;
            }
        }

        /// <summary>
        /// True if the power data comes from a power meter, false if the data is estimated.
        /// </summary>
        [JsonProperty("device_watts")]
        public bool HasPowerMeter { get; set; }

        /// <summary>
        /// Map representing the route of the activity.
        /// </summary>
        //[JsonProperty("map")]
        //public Map Map { get; set; }

        /// <summary>
        /// Meta object of the athlete of this activity.
        /// </summary>
        [JsonProperty("athlete")]
        public AthleteMeta AthleteMeta { get; set; }
    }

    /// <summary>
    /// Seperated added fields from original response class!
    /// </summary>
    public partial class ActivitySummary
    {
        private AthleteSummary _athlete;
        public AthleteSummary Athlete
        {
            get { return _athlete; }
            set { Set(() => Athlete, ref _athlete, value); }
        }
    }
}
