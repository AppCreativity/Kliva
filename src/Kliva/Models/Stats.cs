using Newtonsoft.Json;

namespace Kliva.Models
{
    /// <summary>
    /// Run totals of the past four weeks.
    /// </summary>
    public class RecentRunTotals
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
    }

    /// <summary>
    /// This class contains statistics about your recent rides.
    /// </summary>
    public class RecentRideTotals
    {
        /// <summary>
        /// The number of rides.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// The total distance.
        /// </summary>
        [JsonProperty("distance")]
        public double Distance { get; set; }

        /// <summary>
        /// The total time moved.
        /// </summary>
        [JsonProperty("moving_time")]
        public int MovingTime { get; set; }

        /// <summary>
        /// The total time elapsed.
        /// </summary>
        [JsonProperty("elapsed_time")]
        public int ElapsedTime { get; set; }

        /// <summary>
        /// The total elevation gained.
        /// </summary>
        [JsonProperty("elevation_gain")]
        public double ElevationGain { get; set; }

        /// <summary>
        /// The number of achievements.
        /// </summary>
        [JsonProperty("achievement_count")]
        public int AchievementCount { get; set; }
    }

    /// <summary>
    /// This class contains all the statistics for cycling in this year.
    /// </summary>
    public class YearToDateRideTotals
    {
        /// <summary>
        /// The number of runs.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// The Distance.
        /// </summary>
        [JsonProperty("distance")]
        public int Distance { get; set; }

        /// <summary>
        /// The total time you were moving.
        /// </summary>
        [JsonProperty("moving_time")]
        public int MovingTime { get; set; }

        /// <summary>
        /// The total elapsed time.
        /// </summary>
        [JsonProperty("elapsed_time")]
        public int ElapsedTime { get; set; }

        /// <summary>
        /// The total elevation gain.
        /// </summary>
        [JsonProperty("elevation_gain")]
        public int ElevationGain { get; set; }
    }

    /// <summary>
    /// This class contains all the statistics for running in this year.
    /// </summary>
    public class YearToDateRunTotals
    {
        /// <summary>
        /// The number of runs.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// The Distance.
        /// </summary>
        [JsonProperty("distance")]
        public int Distance { get; set; }

        /// <summary>
        /// The total time you were moving.
        /// </summary>
        [JsonProperty("moving_time")]
        public int MovingTime { get; set; }

        /// <summary>
        /// The total elapsed time.
        /// </summary>
        [JsonProperty("elapsed_time")]
        public int ElapsedTime { get; set; }

        /// <summary>
        /// The total elevation gain.
        /// </summary>
        [JsonProperty("elevation_gain")]
        public int ElevationGain { get; set; }
    }

    /// <summary>
    /// This class represents the datra of all your rides.
    /// </summary>
    public class AllRideTotals
    {
        /// <summary>
        /// The total count of all your rides.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// The cumulative distance of all your rides.
        /// </summary>
        [JsonProperty("distance")]
        public int Distance { get; set; }

        /// <summary>
        /// The cumulative moving time of all your rides.
        /// </summary>
        [JsonProperty("moving_time")]
        public int MovingTime { get; set; }

        /// <summary>
        /// The cumulative elapsed time of all your rides.
        /// </summary>
        [JsonProperty("elapsed_time")]
        public int ElapsedTime { get; set; }

        /// <summary>
        /// The cumulative elevation gain of all your rides.
        /// </summary>
        [JsonProperty("elevation_gain")]
        public int ElevationGain { get; set; }
    }

    /// <summary>
    /// This class represents the data of all your runs.
    /// </summary>
    public class AllRunTotals
    {
        /// <summary>
        /// The number of total runs.
        /// </summary>
        [JsonProperty("count")]
        public int Count { get; set; }

        /// <summary>
        /// The cumulative distance of all your runs.
        /// </summary>
        [JsonProperty("distance")]
        public int Distance { get; set; }

        /// <summary>
        /// The cumulative moving time of all your runs.
        /// </summary>
        [JsonProperty("moving_time")]
        public int MovingTime { get; set; }

        /// <summary>
        /// The cumulative elapsed time of all your runs.
        /// </summary>
        [JsonProperty("elapsed_time")]
        public int ElapsedTime { get; set; }

        /// <summary>
        /// The cumulative elevation gain of all your runs.
        /// </summary>
        [JsonProperty("elevation_gain")]
        public int ElevationGain { get; set; }
    }

    /// <summary>
    /// This class contains all statistics of an athlete.
    /// </summary>
    public class Stats
    {
        /// <summary>
        /// The distance of your biggest ride.
        /// </summary>
        [JsonProperty("biggest_ride_distance")]
        public double BiggestRideDistance { get; set; }

        /// <summary>
        /// The most elevation gain in a single ride.
        /// </summary>
        [JsonProperty("biggest_climb_elevation_gain")]
        public double BiggestClimbElevationGain { get; set; }

        /// <summary>
        /// Statistics about your recent rides.
        /// </summary>
        [JsonProperty("recent_ride_totals")]
        public RecentRideTotals RecentRideTotals { get; set; }

        /// <summary>
        /// Statistics about your recent runs.
        /// </summary>
        [JsonProperty("recent_run_totals")]
        public RecentRunTotals RecentRunTotals { get; set; }

        /// <summary>
        /// Ride statistics from this year.
        /// </summary>
        [JsonProperty("ytd_ride_totals")]
        public YearToDateRideTotals YearToDateRideTotals { get; set; }

        /// <summary>
        /// Run statistics from this year.
        /// </summary>
        [JsonProperty("ytd_run_totals")]
        public YearToDateRunTotals YearToDateRunTotals { get; set; }

        /// <summary>
        /// Total ride statistics.
        /// </summary>
        [JsonProperty("all_ride_totals")]
        public AllRideTotals RideTotals { get; set; }

        /// <summary>
        /// Total run statistics.
        /// </summary>
        [JsonProperty("all_run_totals")]
        public AllRunTotals RunTotals { get; set; }
    }
}
