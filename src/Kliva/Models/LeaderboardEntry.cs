using System;
using Kliva.Helpers;
using Newtonsoft.Json;
using Kliva.Models.Interfaces;

namespace Kliva.Models
{
    /// <summary>
    /// This class contains information about a single leaderboard entry.
    /// </summary>
    public partial class LeaderboardEntry
    {
        /// <summary>
        /// The full name of the athlete.
        /// </summary>
        [JsonProperty("athlete_name")]
        public string AthleteName { get; set; }

        /// <summary>
        /// The athlete id. Use this id to load additional information about the athlete.
        /// </summary>
        [JsonProperty("athlete_id")]
        public long AthleteId { get; set; }

        /// <summary>
        /// The athlete's gender.
        /// </summary>
        [JsonProperty("athlete_gender")]
        public string AthleteGender { get; set; }

        /// <summary>
        /// The average heartrate.
        /// </summary>
        [JsonProperty("average_hr")]
        public float? AverageHeartrate { get; set; }

        /// <summary>
        /// The average power.
        /// </summary>
        [JsonProperty("average_watts")]
        public float? AveragePower { get; set; }

        /// <summary>
        /// The distance ridden on this single attempt.
        /// </summary>
        [JsonProperty("distance")]
        public float Distance { get; set; }

        /// <summary>
        /// The elapsed time.
        /// </summary>
        [JsonProperty("elapsed_time")]
        public int ElapsedTime { get; set; }

        /// <summary>
        /// The moving time.
        /// </summary>
        [JsonProperty("moving_time")]
        public int MovingTime { get; set; }

        /// <summary>
        /// The start date.
        /// </summary>
        [JsonProperty("start_date")]
        public string StartDate { get; set; }

        /// <summary>
        /// Local start date.
        /// </summary>
        [JsonProperty("start_date_local")]
        public string StartDateLocal { get; set; }

        /// <summary>
        /// Returns the StartDate-Property as a DateTime object.
        /// </summary>
        public DateTime DateTimeStart => DateTime.Parse(StartDate);

        /// <summary>
        /// Returns the StartDateLocal-Property as a DateTime object.
        /// </summary>
        public DateTime DateTimeStartLocal => DateTime.Parse(StartDateLocal);

        /// <summary>
        /// Returns the moving time as a TimeSpan object rather than an int value.
        /// </summary>
        public TimeSpan MovingTimeSpan => TimeSpan.FromSeconds(MovingTime);

        /// <summary>
        /// Returns the elapsed time as a TimeSpan object rather than an int value.
        /// </summary>
        public TimeSpan ElapsedTimeSpan => TimeSpan.FromSeconds(ElapsedTime);

        /// <summary>
        /// The activity id. use this id to load additional information about the activity.
        /// </summary>
        [JsonProperty("activity_id")]
        public long ActivityId { get; set; }

        /// <summary>
        /// The effort id. Use this id to load additional information about the segment effort.
        /// </summary>
        [JsonProperty("effort_id")]
        public long EffortId { get; set; }

        /// <summary>
        /// The rank of athlete.
        /// </summary>
        [JsonProperty("rank")]
        public int Rank { get; set; }

        /// <summary>
        /// Url to a picture of the athlete. If not null or empty, you can use the ImageLoader to load the picture from the Url.
        /// </summary>
        [JsonProperty("athlete_profile")]
        public string AthleteProfile { get; set; }

        /// <summary>
        /// Returns the total seconds in a more convenient TimeSpan object.
        /// </summary>
        public TimeSpan Time => TimeSpan.FromSeconds(ElapsedTime);

        /// <summary>
        /// Returns the information about the entry as a string.
        /// </summary>
        /// <returns>Entry as string.</returns>
        public override string ToString()
        {
            return $"{Rank}:\t{Time.Hours.ToString("D2")}:{Time.Minutes.ToString("D2")}:{Time.Seconds.ToString("D2")}\t{AthleteName}";
        }
    }

    /// <summary>
    /// Separated added fields from original response class!
    /// </summary>
    public partial class LeaderboardEntry : BaseClass, ISupportUserMeasurementUnits
    {
        public Segment Segment { get; set; }

        public float AverageSpeed => Distance / ElapsedTime;

        public float AverageHeartrateDisplay => AverageHeartrate ?? 0;

        public DistanceUnitType MeasurementUnit
        {
            get;
            set;
        }

        public void SetUserMeasurementUnits(DistanceUnitType measurementUnit)
        {
            MeasurementUnit = measurementUnit;

            bool isMetric = MeasurementHelper.IsMetric(MeasurementUnit);
            var speedUnit = MeasurementHelper.GetSpeedUnit(isMetric);
            var distanceUnitType = MeasurementHelper.GetDistanceUnitType(isMetric);

            AverageSpeedUserMeasurementUnit = new UserMeasurementUnitMetric(AverageSpeed, SpeedUnit.MetresPerSecond, speedUnit);
            DistanceUserMeasurementUnit = new UserMeasurementUnitMetric(Distance, DistanceUnitType.Metres, distanceUnitType);
        }

        public UserMeasurementUnitMetric DistanceUserMeasurementUnit
        {
            get;
            private set;
        }

        public UserMeasurementUnitMetric AverageSpeedUserMeasurementUnit
        {
            get;
            private set;
        }
    }
}
