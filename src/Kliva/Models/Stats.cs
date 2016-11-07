using Kliva.Helpers;
using Kliva.Models.Interfaces;
using Newtonsoft.Json;

namespace Kliva.Models
{
    /// <summary>
    /// Run totals of the past four weeks.
    /// </summary>
    public class StatTotals : ISupportUserMeasurementUnits
    {
        /// <summary>
        /// Number of activities.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// Total distance.
        /// </summary>
        [JsonProperty("distance")]
        public double Distance { get; set; }

        /// <summary>
        /// Moving time.
        /// </summary>
        [JsonProperty("moving_time")]
        public int MovingTime { get; set; }

        /// <summary>
        /// Elapsed time.
        /// </summary>
        [JsonProperty("elapsed_time")]
        public int ElapsedTime { get; set; }

        /// <summary>
        /// Elevation gain in metres.
        /// </summary>
        [JsonProperty("elevation_gain")]
        public double ElevationGain { get; set; }

        /// <summary>
        /// Achievement count.
        /// </summary>
        [JsonProperty("achievement_count")]
        public int AchievementCount { get; set; }

        public UserMeasurementUnitMetric TotalDistanceUserMeasurementUnit
        {
            get;
            private set;
        }

        public UserMeasurementUnitMetric ElevationGainUserMeasurementUnit
        {
            get;
            private set;
        }

        public DistanceUnitType MeasurementUnit
        {
            get;
            private set;
        }

        public void SetUserMeasurementUnits(DistanceUnitType measurementUnit)
        {
            MeasurementUnit = measurementUnit;
            bool isMetric = MeasurementHelper.IsMetric(MeasurementUnit);
            var elevationDistanceUnitType = MeasurementHelper.GetElevationUnitType(isMetric);
            var distanceUnitType = MeasurementHelper.GetDistanceUnitType(isMetric);

            TotalDistanceUserMeasurementUnit = new UserMeasurementUnitMetric((float)Distance, DistanceUnitType.Metres, distanceUnitType);
            ElevationGainUserMeasurementUnit = new UserMeasurementUnitMetric((float)ElevationGain, DistanceUnitType.Metres, elevationDistanceUnitType);
        }
    }

    /// <summary>
    /// This class contains all statistics of an athlete.
    /// </summary>
    public partial class Stats : BaseClass
    {
        /// <summary>
        /// The distance of your biggest ride.
        /// </summary>
        [JsonProperty("biggest_ride_distance")]
        public float? BiggestRideDistance { get; set; }

        /// <summary>
        /// The most elevation gain in a single ride.
        /// </summary>
        [JsonProperty("biggest_climb_elevation_gain")]
        public float? BiggestClimbElevationGain { get; set; }

        /// <summary>
        /// Statistics about your recent rides.
        /// </summary>
        [JsonProperty("recent_ride_totals")]
        public StatTotals RecentRideTotals { get; set; }

        /// <summary>
        /// Statistics about your recent runs.
        /// </summary>
        [JsonProperty("recent_run_totals")]
        public StatTotals RecentRunTotals { get; set; }

        /// <summary>
        /// Ride statistics from this year.
        /// </summary>
        [JsonProperty("ytd_ride_totals")]
        public StatTotals YearToDateRideTotals { get; set; }

        /// <summary>
        /// Run statistics from this year.
        /// </summary>
        [JsonProperty("ytd_run_totals")]
        public StatTotals YearToDateRunTotals { get; set; }

        /// <summary>
        /// Total ride statistics.
        /// </summary>
        [JsonProperty("all_ride_totals")]
        public StatTotals RideTotals { get; set; }

        /// <summary>
        /// Total run statistics.
        /// </summary>
        [JsonProperty("all_run_totals")]
        public StatTotals RunTotals { get; set; }
    }

    /// <summary>
    /// Separated added fields from original response class!
    /// </summary>
    public partial class Stats : ISupportUserMeasurementUnits
    {
        public DistanceUnitType MeasurementUnit
        {
            get;
            private set;
        }

        public void SetUserMeasurementUnits(DistanceUnitType measurementUnit)
        {
            MeasurementUnit = measurementUnit;

            bool isMetric = MeasurementHelper.IsMetric(MeasurementUnit);
            var elevationDistanceUnitType = MeasurementHelper.GetElevationUnitType(isMetric);
            var distanceUnitType = MeasurementHelper.GetDistanceUnitType(isMetric);

            BiggestRideDistanceUserMeasurementUnit = new UserMeasurementUnitMetric(BiggestRideDistance ?? 0, DistanceUnitType.Metres, distanceUnitType);
            BiggestClimbElevationGainUserMeasurementUnit = new UserMeasurementUnitMetric(BiggestClimbElevationGain ?? 0, DistanceUnitType.Metres, elevationDistanceUnitType);
        }

        public UserMeasurementUnitMetric BiggestRideDistanceUserMeasurementUnit
        {
            get;
            private set;
        }

        public UserMeasurementUnitMetric BiggestClimbElevationGainUserMeasurementUnit
        {
            get;
            private set;
        }
    }
}
