using Kliva.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Kliva.Services.Interfaces;
using Microsoft.Practices.ServiceLocation;

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
        private string _Name;
        [JsonProperty("name")]
        public string Name
        {
            get { return _Name; }
            set { Set(() => Name, ref _Name, value); }
        }


        /// <summary>
        /// The external id of the activity. Provided upon upload. Can not be changed.
        /// </summary>
        private string _ExternalId;
        [JsonProperty("external_id")]
        public string ExternalId
        {
            get { return _ExternalId; }
            set { Set(() => ExternalId, ref _ExternalId, value); }
        }


        /// <summary>
        /// The type of the activity.
        /// </summary>
        private string __type;
        [JsonProperty("type")]
        private string _type
        {
            get { return __type; }
            set
            {
                Set(() => _type, ref __type, value);
                RaisePropertyChanged("ActivityType");
                RaisePropertyChanged(() => TypeImage);
            }
        }


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
        private float _Distance;
        [JsonProperty("distance")]
        public float Distance
        {
            get { return _Distance; }
            set
            {
                Set(() => Distance, ref _Distance, value);
                RaisePropertyChanged(() => DistanceFormatted);
            }
        }


        /// <summary>
        /// Time in movement in seconds.
        /// </summary>
        private int _MovingTime;
        [JsonProperty("moving_time")]
        public int MovingTime
        {
            get { return _MovingTime; }
            set
            {
                Set(() => MovingTime, ref _MovingTime, value);
                RaisePropertyChanged(() => MovingTimeSpan);
            }
        }


        /// <summary>
        /// The elapsed time in seconds.
        /// </summary>
        private int _ElapsedTime;
        [JsonProperty("elapsed_time")]
        public int ElapsedTime
        {
            get { return _ElapsedTime; }
            set
            {
                Set(() => ElapsedTime, ref _ElapsedTime, value);
                RaisePropertyChanged(() => ElapsedTimeSpan);
            }
        }


        /// <summary>
        /// The total elevation gain in meters.
        /// </summary>
        private float _ElevationGain;
        [JsonProperty("total_elevation_gain")]
        public float ElevationGain
        {
            get { return _ElevationGain; }
            set
            {
                Set(() => ElevationGain, ref _ElevationGain, value);
                RaisePropertyChanged(() => ElevationGainFormatted);
            }
        }


        /// <summary>
        /// True if the currently authenticated athlete has kudoed this activity.
        /// </summary>
        private bool _HasKudoed;
        [JsonProperty("has_kudoed")]
        public bool HasKudoed
        {
            get { return _HasKudoed; }
            set { Set(() => HasKudoed, ref _HasKudoed, value); }
        }


        /// <summary>
        /// The athlete's average heartrate during this activity.
        /// </summary>
        private float _AverageHeartrate;
        [JsonProperty("average_heartrate")]
        public float AverageHeartrate
        {
            get { return _AverageHeartrate; }
            set { Set(() => AverageHeartrate, ref _AverageHeartrate, value); }
        }


        /// <summary>
        /// The athlete's max heartrate.
        /// </summary>
        private float _MaxHeartrate;
        [JsonProperty("max_heartrate")]
        public float MaxHeartrate
        {
            get { return _MaxHeartrate; }
            set { Set(() => MaxHeartrate, ref _MaxHeartrate, value); }
        }


        /// <summary>
        /// Only present if activity is owned by authenticated athlete, returns 0 if not truncated by privacy zones.
        /// </summary>
        private int? _Truncated;
        [JsonProperty("truncated")]
        public int? Truncated
        {
            get { return _Truncated; }
            set { Set(() => Truncated, ref _Truncated, value); }
        }

        /// <summary>
        /// The city where this activity was started.
        /// </summary>
        private string _City;
        [JsonProperty("city")]
        public string City
        {
            get { return _City; }
            set { Set(() => City, ref _City, value); }
        }


        /// <summary>
        /// The state where this activity was started.
        /// </summary>
        private string _State;
        [JsonProperty("state")]
        public string State
        {
            get { return _State; }
            set { Set(() => State, ref _State, value); }
        }


        /// <summary>
        /// The country where this activity was started.
        /// </summary>
        private string _Country;
        [JsonProperty("country")]
        public string Country
        {
            get { return _Country; }
            set { Set(() => Country, ref _Country, value); }
        }


        /// <summary>
        /// The id of the gear used.
        /// </summary>
        private string _GearId;
        [JsonProperty("gear_id")]
        public string GearId
        {
            get { return _GearId; }
            set { Set(() => GearId, ref _GearId, value); }
        }


        /// <summary>
        /// The average speed in meters per seconds.
        /// </summary>
        private float _AverageSpeed;
        [JsonProperty("average_speed")]
        public float AverageSpeed
        {
            get { return _AverageSpeed; }
            set
            {
                Set(() => AverageSpeed, ref _AverageSpeed, value);
                RaisePropertyChanged(() => AverageSpeedFormatted);
            }
        }


        /// <summary>
        /// The max speed in meters per second.
        /// </summary>
        private float _MaxSpeed;
        [JsonProperty("max_speed")]
        public float MaxSpeed
        {
            get { return _MaxSpeed; }
            set
            {
                Set(() => MaxSpeed, ref _MaxSpeed, value);
                RaisePropertyChanged(() => MaxSpeedFormatted);
            }
        }


        /// <summary>
        /// The average cadence. Only returned if activity is a bike ride.
        /// </summary>
        private float _AverageCadence;
        [JsonProperty("average_cadence")]
        public float AverageCadence
        {
            get { return _AverageCadence; }
            set { Set(() => AverageCadence, ref _AverageCadence, value); }
        }


        /// <summary>
        /// The average temperature during this activity. Only returned if data is provided upon upload.
        /// </summary>
        private float _AverageTemperature;
        [JsonProperty("average_temp")]
        public float AverageTemperature
        {
            get { return _AverageTemperature; }
            set { Set(() => AverageTemperature, ref _AverageTemperature, value); }
        }


        /// <summary>
        /// The average power during this activity. Only returned if data is provided upon upload.
        /// </summary>
        private float _AveragePower;
        [JsonProperty("average_watts")]
        public float AveragePower
        {
            get { return _AveragePower; }
            set { Set(() => AveragePower, ref _AveragePower, value); }
        }


        /// <summary>
        /// Kilojoules. Rides only.
        /// </summary>
        private float _Kilojoules;
        [JsonProperty("kilojoules")]
        public float Kilojoules
        {
            get { return _Kilojoules; }
            set { Set(() => Kilojoules, ref _Kilojoules, value); }
        }


        /// <summary>
        /// True if the activity was recorded on a stationary trainer.
        /// </summary>
        private bool _IsTrainer;
        [JsonProperty("trainer")]
        public bool IsTrainer
        {
            get { return _IsTrainer; }
            set { Set(() => IsTrainer, ref _IsTrainer, value); }
        }


        /// <summary>
        /// True if activity is a a commute.
        /// </summary>
        private bool _IsCommute;
        [JsonProperty("commute")]
        public bool IsCommute
        {
            get { return _IsCommute; }
            set { Set(() => IsCommute, ref _IsCommute, value); }
        }


        /// <summary>
        /// True if the ride was crated manually.
        /// </summary>
        private bool _IsManual;
        [JsonProperty("manual")]
        public bool IsManual
        {
            get { return _IsManual; }
            set { Set(() => IsManual, ref _IsManual, value); }
        }


        /// <summary>
        /// True if the activity is private.
        /// </summary>
        private bool _IsPrivate;
        [JsonProperty("private")]
        public bool IsPrivate
        {
            get { return _IsPrivate; }
            set { Set(() => IsPrivate, ref _IsPrivate, value); }
        }


        /// <summary>
        /// True if the activity was flagged.
        /// </summary>
        private bool _IsFlagged;
        [JsonProperty("flagged")]
        public bool IsFlagged
        {
            get { return _IsFlagged; }
            set { Set(() => IsFlagged, ref _IsFlagged, value); }
        }


        /// <summary>
        /// Achievement count.
        /// </summary>
        private int _AchievementCount;
        [JsonProperty("achievement_count")]
        public int AchievementCount
        {
            get { return _AchievementCount; }
            set { Set(() => AchievementCount, ref _AchievementCount, value); }
        }


        /// <summary>
        /// Activity's kudos count.
        /// </summary>
        private int _KudosCount;
        [JsonProperty("kudos_count")]
        public int KudosCount
        {
            get { return _KudosCount; }
            set { Set(() => KudosCount, ref _KudosCount, value); }
        }


        /// <summary>
        /// Activity's comment count.
        /// </summary>
        private int _CommentCount;
        [JsonProperty("comment_count")]
        public int CommentCount
        {
            get { return _CommentCount; }
            set { Set(() => CommentCount, ref _CommentCount, value); }
        }


        /// <summary>
        /// Number of athletes on this activity.
        /// </summary>
        private int _AthleteCount;
        [JsonProperty("athlete_count")]
        public int AthleteCount
        {
            get { return _AthleteCount; }
            set { Set(() => AthleteCount, ref _AthleteCount, value); }
        }


        /// <summary>
        /// Number of photos.
        /// </summary>
        private int _PhotoCount;
        [JsonProperty("photo_count")]
        public int PhotoCount
        {
            get { return _PhotoCount; }
            set { Set(() => PhotoCount, ref _PhotoCount, value); }
        }


        /// <summary>
        /// Start date of the activity.
        /// </summary>
        private string _StartDate;
        [JsonProperty("start_date")]
        public string StartDate
        {
            get { return _StartDate; }
            set
            {
                Set(() => StartDate, ref _StartDate, value);
                RaisePropertyChanged(() => DateTimeStart);
            }
        }

        private DateTime _endDate;
        /// <summary>
        /// For sorting purposes only
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                if (_endDate == new DateTime() && !string.IsNullOrEmpty(StartDate))
                {
                    DateTime start = DateTime.Parse(StartDate);
                    _endDate = start.AddSeconds(ElapsedTime);
                }
                return _endDate;
            }
        }


        /// <summary>
        /// Local start date of the activity.
        /// </summary>
        private string _StartDateLocal;
        [JsonProperty("start_date_local")]
        public string StartDateLocal
        {
            get { return _StartDateLocal; }
            set
            {
                Set(() => StartDateLocal, ref _StartDateLocal, value);
                RaisePropertyChanged(() => DateTimeStartLocal);
            }
        }


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
        private string _TimeZone;
        [JsonProperty("timezone")]
        public string TimeZone
        {
            get { return _TimeZone; }
            set { Set(() => TimeZone, ref _TimeZone, value); }
        }


        /// <summary>
        /// Coordinate where the activity was started.
        /// </summary>

        private List<double> _StartPoint;
        [JsonProperty("start_latlng")]
        public List<double> StartPoint
        {
            get { return _StartPoint; }
            set
            {
                Set(() => StartPoint, ref _StartPoint, value);
                RaisePropertyChanged(() => StartLatitude);
                RaisePropertyChanged(() => StartLongitude);
            }
        }

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
        private int _WeightedAverageWatts;
        [JsonProperty("weighted_average_watts")]
        public int WeightedAverageWatts
        {
            get { return _WeightedAverageWatts; }
            set { Set(() => WeightedAverageWatts, ref _WeightedAverageWatts, value); }
        }


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
        private List<double> _endPoint;
        [JsonProperty("end_latlng")]
        public List<double> EndPoint
        {
            get { return _endPoint; }
            set
            {
                Set(() => EndPoint, ref _endPoint, value);
                RaisePropertyChanged(() => EndLatitude);
                RaisePropertyChanged(() => EndLongitude);
            }
        }



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
        private bool _HasPowerMeter;
        [JsonProperty("device_watts")]
        public bool HasPowerMeter
        {
            get { return _HasPowerMeter; }
            set { Set(() => HasPowerMeter, ref _HasPowerMeter, value); }
        }


        /// <summary>
        /// Map representing the route of the activity.
        /// </summary>
        private Map _Map;
        [JsonProperty("map")]
        public Map Map
        {
            get { return _Map; }
            set { Set(() => Map, ref _Map, value); }
        }


        private AthleteMeta _AthleteMeta;
        /// <summary>
        /// Meta object of the athlete of this activity.
        /// </summary>
        [JsonProperty("athlete")]
        public AthleteMeta AthleteMeta
        {
            get { return _AthleteMeta; }
            set
            {
                _AthleteMeta = value;
                ConsolidateWithCache(value);
            }
        }
    }

    /// <summary>
    /// Separated added fields from original response class!
    /// </summary>
    public partial class ActivitySummary : IKey
    {
        private AthleteSummary _athlete;
        public AthleteSummary Athlete
        {
            get { return _athlete; }
            set { Set(() => Athlete, ref _athlete, value); }
        }

        private IList<ActivitySummary> _relatedActivities;
        public IList<ActivitySummary> RelatedActivities
        {
            get { return _relatedActivities; }
            set { Set(() => RelatedActivities, ref _relatedActivities, value); }
        }

        private List<Photo> _allPhotos;
        public List<Photo> AllPhotos
        {
            get { return _allPhotos; }
            set { Set(() => AllPhotos, ref _allPhotos, value); }
        }

        private List<AthleteSummary> _kudos;
        public List<AthleteSummary> Kudos
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
        private int _TotalPhotoCount;
        [JsonProperty("total_photo_count")]
        public int TotalPhotoCount
        {
            get { return _TotalPhotoCount; }
            set { Set(() => TotalPhotoCount, ref _TotalPhotoCount, value); }
        }


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
                            break;
                        case ActivityType.Walk:
                            _typeImage = "";
                            break;
                        case ActivityType.Run:
                            _typeImage = "";
                            break;
                        case ActivityType.AlpineSki:
                        case ActivityType.BackcountrySki:
                        case ActivityType.NordicSki:
                        case ActivityType.Snowboard:
                            _typeImage = "";
                            break;
                        default:
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
                switch (DistanceUnit)
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
        public int OtherAthleteCount => AthleteCount - 1;

        long IKey.Key => Id;

        private void ConsolidateWithCache(AthleteMeta meta)
        {
            // TODO refactor servicelocator out of model
            IStravaService stravaService = ServiceLocator.Current.GetInstance<IStravaService>();

            AthleteSummary summary = stravaService.ConsolidateWithCache(meta);
            Athlete = summary;
        }

        public void MergeInData(ActivitySummary newData)
        {
            this.Name = newData.Name;
            this.ExternalId = newData.ExternalId;
            this._type = newData._type;
            this.Distance = newData.Distance;
            this.MovingTime = newData.MovingTime;
            this.ElapsedTime = newData.ElapsedTime;
            this.ElevationGain = newData.ElevationGain;
            this.HasKudoed = newData.HasKudoed;
            this.AverageHeartrate = newData.AverageHeartrate;
            this.MaxHeartrate = newData.MaxHeartrate;
            this.Truncated = newData.Truncated;
            this.City = newData.City;
            this.State = newData.State;
            this.Country = newData.Country;
            this.GearId = newData.GearId;
            this.AverageSpeed = newData.AverageSpeed;
            this.MaxSpeed = newData.MaxSpeed;
            this.AverageCadence = newData.AverageCadence;
            this.AverageTemperature = newData.AverageTemperature;
            this.AveragePower = newData.AveragePower;
            this.Kilojoules = newData.Kilojoules;
            this.IsTrainer = newData.IsTrainer;
            this.IsCommute = newData.IsCommute;
            this.IsManual = newData.IsManual;
            this.IsPrivate = newData.IsPrivate;
            this.IsFlagged = newData.IsFlagged;
            this.AchievementCount = newData.AchievementCount;
            this.KudosCount = newData.KudosCount;
            this.CommentCount = newData.CommentCount;
            this.AthleteCount = newData.AthleteCount;
            this.PhotoCount = newData.PhotoCount;
            this.StartDate = newData.StartDate;
            this.StartDateLocal = newData.StartDateLocal;
            this.TimeZone = newData.TimeZone;
            this.StartPoint = newData.StartPoint;
            this.WeightedAverageWatts = newData.WeightedAverageWatts;
            this.EndPoint = newData.EndPoint;
            this.HasPowerMeter = newData.HasPowerMeter;
            this.Map = newData.Map;
        }
    }
}
