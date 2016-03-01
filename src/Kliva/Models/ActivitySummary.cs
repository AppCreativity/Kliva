using Kliva.Helpers;
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
        [JsonProperty("map")]
        public Map Map { get; set; }

        /// <summary>
        /// Meta object of the athlete of this activity.
        /// </summary>
        [JsonProperty("athlete")]
        public AthleteMeta AthleteMeta { get; set; }
    }

    /// <summary>
    /// Separated added fields from original response class!
    /// </summary>
    public partial class ActivitySummary
    {
        private AthleteSummary _athlete;
        public AthleteSummary Athlete
        {
            get { return _athlete; }
            set { Set(() => Athlete, ref _athlete, value); }
        }

        private List<Photo> _allPhotos;
        public List<Photo> AllPhotos
        {
            get { return _allPhotos; }
            set { Set(() => AllPhotos, ref _allPhotos, value); }
        }

        private List<Athlete> _kudos;
        public List<Athlete> Kudos
        {
            get { return _kudos; }
            set { Set(() => Kudos, ref _kudos, value); }
        }

        private List<Comment> _comments;
        public List<Comment> Comments
        {
            get { return _comments; }
            set { Set(() => Comments, ref _comments, value); }
        }

        /// <summary>
        /// Number of photos ( Instagram & Strava ).
        /// </summary>
        [JsonProperty("total_photo_count")]
        public int TotalPhotoCount { get; set; }

        private string _typeImage;
        public string TypeImage
        {
            get
            {
                if (string.IsNullOrEmpty(_typeImage))
                {
                    switch (this.Type)
                    {
                        case ActivityType.Ride:
                        case ActivityType.EBikeRide:
                        case ActivityType.VirtualRide:
                            _typeImage = "";
                            //_typeImage = "M5,20.5A3.5,3.5 0 0,1 1.5,17A3.5,3.5 0 0,1 5,13.5A3.5,3.5 0 0,1 8.5,17A3.5,3.5 0 0,1 5,20.5M5,12A5,5 0 0,0 0,17A5,5 0 0,0 5,22A5,5 0 0,0 10,17A5,5 0 0,0 5,12M14.8,10H19V8.2H15.8L13.86,4.93C13.57,4.43 13,4.1 12.4,4.1C11.93,4.1 11.5,4.29 11.2,4.6L7.5,8.29C7.19,8.6 7,9 7,9.5C7,10.13 7.33,10.66 7.85,10.97L11.2,13V18H13V11.5L10.75,9.85L13.07,7.5M19,20.5A3.5,3.5 0 0,1 15.5,17A3.5,3.5 0 0,1 19,13.5A3.5,3.5 0 0,1 22.5,17A3.5,3.5 0 0,1 19,20.5M19,12A5,5 0 0,0 14,17A5,5 0 0,0 19,22A5,5 0 0,0 24,17A5,5 0 0,0 19,12M16,4.8C17,4.8 17.8,4 17.8,3C17.8,2 17,1.2 16,1.2C15,1.2 14.2,2 14.2,3C14.2,4 15,4.8 16,4.8Z";
                            break;
                        case ActivityType.Walk:
                            _typeImage = "";
                            break;
                        case ActivityType.Run:
                            //_typeImage = "M17.12,10L16.04,8.18L15.31,11.05L17.8,15.59V22H16V17L13.67,13.89L12.07,18.4L7.25,20.5L6.2,19L10.39,16.53L12.91,6.67L10.8,7.33V11H9V5.8L14.42,4.11L14.92,4.03C15.54,4.03 16.08,4.37 16.38,4.87L18.38,8.2H22V10H17.12M17,3.8C16,3.8 15.2,3 15.2,2C15.2,1 16,0.2 17,0.2C18,0.2 18.8,1 18.8,2C18.8,3 18,3.8 17,3.8M7,9V11H4A1,1 0 0,1 3,10A1,1 0 0,1 4,9H7M9.25,13L8.75,15H5A1,1 0 0,1 4,14A1,1 0 0,1 5,13H9.25M7,5V7H3A1,1 0 0,1 2,6A1,1 0 0,1 3,5H7Z";
                            _typeImage = "";
                            break;
                        case ActivityType.AlpineSki:
                        case ActivityType.BackcountrySki:
                        case ActivityType.NordicSki:
                        case ActivityType.Snowboard:
                            //_typeImage = "M6,14A1,1 0 0,1 7,15A1,1 0 0,1 6,16A5,5 0 0,1 1,11A5,5 0 0,1 6,6C7,3.65 9.3,2 12,2C15.43,2 18.24,4.66 18.5,8.03L19,8A4,4 0 0,1 23,12A4,4 0 0,1 19,16H18A1,1 0 0,1 17,15A1,1 0 0,1 18,14H19A2,2 0 0,0 21,12A2,2 0 0,0 19,10H17V9A5,5 0 0,0 12,4C9.5,4 7.45,5.82 7.06,8.19C6.73,8.07 6.37,8 6,8A3,3 0 0,0 3,11A3,3 0 0,0 6,14M7.88,18.07L10.07,17.5L8.46,15.88C8.07,15.5 8.07,14.86 8.46,14.46C8.85,14.07 9.5,14.07 9.88,14.46L11.5,16.07L12.07,13.88C12.21,13.34 12.76,13.03 13.29,13.17C13.83,13.31 14.14,13.86 14,14.4L13.41,16.59L15.6,16C16.14,15.86 16.69,16.17 16.83,16.71C16.97,17.24 16.66,17.79 16.12,17.93L13.93,18.5L15.54,20.12C15.93,20.5 15.93,21.15 15.54,21.54C15.15,21.93 14.5,21.93 14.12,21.54L12.5,19.93L11.93,22.12C11.79,22.66 11.24,22.97 10.71,22.83C10.17,22.69 9.86,22.14 10,21.6L10.59,19.41L8.4,20C7.86,20.14 7.31,19.83 7.17,19.29C7.03,18.76 7.34,18.21 7.88,18.07Z";
                            _typeImage = "";
                            break;
                        default:
                            //_typeImage = "M14,6L10.25,11L13.1,14.8L11.5,16C9.81,13.75 7,10 7,10L1,18H23L14,6Z";
                            _typeImage = "";
                            break;                            
                    }
                }

                return _typeImage;
            }
        }

        private DistanceUnitType _distanceUnit;
        public DistanceUnitType DistanceUnit
        {
            get { return _distanceUnit; }
            set
            {
                Set(() => DistanceUnit, ref _distanceUnit, value);
                RaisePropertyChanged(() => DistanceFormatted);

                //TODO: Glenn - do we need to 'recalculate' other values?
            }
        }

        private SpeedUnit _speedUnit;
        public SpeedUnit SpeedUnit
        {
            get { return _speedUnit; }
            set
            {
                Set(() => SpeedUnit, ref _speedUnit, value);
                RaisePropertyChanged(() => AverageSpeedFormatted);
                RaisePropertyChanged(() => MaxSpeedFormatted);

                //TODO: Glenn - do we need to 'recalculate' other values?
            }
        }

        private DistanceUnitType _elevationUnit;
        public DistanceUnitType ElevationUnit
        {
            get { return _elevationUnit; }
            set
            {
                Set(() => ElevationUnit, ref _elevationUnit, value);
                RaisePropertyChanged(() => ElevationGainFormatted);
            }
        }

        public string DistanceFormatted
        {
            get
            {
                switch (DistanceUnit)
                {
                    case DistanceUnitType.Kilometres:
                        return UnitConverter.ConvertDistance(Distance, DistanceUnitType.Metres, DistanceUnitType.Kilometres).ToString("F1");
                    case DistanceUnitType.Miles:
                        return UnitConverter.ConvertDistance(Distance, DistanceUnitType.Metres, DistanceUnitType.Miles).ToString("F1");
                }

                return null;
            }
        }

        public string AverageSpeedFormatted
        {
            get
            {
                switch (DistanceUnit)
                {
                    case DistanceUnitType.Kilometres:
                        return UnitConverter.ConvertSpeed(AverageSpeed, SpeedUnit.MetresPerSecond, SpeedUnit.KilometresPerHour).ToString("F1");
                    case DistanceUnitType.Miles:
                        return UnitConverter.ConvertSpeed(AverageSpeed, SpeedUnit.MetresPerSecond, SpeedUnit.MilesPerHour).ToString("F1");
                }

                return null;
            }
        }

        public string MaxSpeedFormatted
        {
            get
            {
                switch (DistanceUnit)
                {
                    case DistanceUnitType.Kilometres:
                        return UnitConverter.ConvertSpeed(MaxSpeed, SpeedUnit.MetresPerSecond, SpeedUnit.KilometresPerHour).ToString("F1");
                    case DistanceUnitType.Miles:
                        return UnitConverter.ConvertSpeed(MaxSpeed, SpeedUnit.MetresPerSecond, SpeedUnit.MilesPerHour).ToString("F1");
                }

                return null;
            }
        }

        public string ElevationGainFormatted
        {
            get
            {
                switch(DistanceUnit)
                {
                    case DistanceUnitType.Kilometres:
                        return this.ElevationGain.ToString("F1");
                    case DistanceUnitType.Miles:
                        return UnitConverter.ConvertDistance(ElevationGain, DistanceUnitType.Metres, DistanceUnitType.Feet).ToString("F1");
                }

                return null;
            }
        }

        /// <summary>
        /// AthleteCount = Athlete + other
        /// </summary>
        public int OtherAthleteCount => AthleteCount -1;
    }
}
