namespace Kliva.Models
{
    public enum ActivityTracking
    {
        Idle,
        Recording,
        Paused,
        Finished
    }

    public enum ActivityFeedFilter
    {
        All,
        My,
        Followers,
        Friends // Following
    }

    public enum ActivityRecording
    {
        Cycling,
        Running
    }

    public enum AppTarget
    {
        Desktop,
        Mobile
    }

    public enum ActivityPivots
    {
        Statistics,
        Kudos,
        Comments,
        Segments,
        Photos
    }

    public enum ProfilePivots
    {
        Friends,
        Followers,
        MutualFriends,
        Koms,
        StarredSegments
    }

    public enum DistanceUnitType
    {
        Kilometres,
        Miles,
        Metres,
        Feet
    }

    public enum SpeedUnit
    {
        MetresPerSecond,
        KilometresPerHour,
        MilesPerHour
    }

    public enum DisplayMode
    {
        CompactOverlay,
        Inline
    }

    public enum MenuItemType
    {
        Home,
        Statistics,
        Profile,
        Clubs,
        Settings,
        Empty
    }

    public enum MenuItemFontType
    {
        MDL2,
        Material
    }

    /// <summary>
    /// This enum is used to return a more expressive bike type instead of just a number.
    /// </summary>
    public enum BikeType
    {
        /// <summary>
        /// The bike is a mountain bike.
        /// </summary>
        Mountain,
        /// <summary>
        /// The bike is a cross bike.
        /// </summary>
        Cross,
        /// <summary>
        /// The bike is a road bike.
        /// </summary>
        Road,
        /// <summary>
        /// The bike is a time trial bike.
        /// </summary>
        Timetrial
    }

    /// <summary>
    /// The type of an activity.
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        /// Ride
        /// </summary>
        Ride,
        /// <summary>
        /// Run
        /// </summary>
        Run,
        /// <summary>
        /// Swim
        /// </summary>
        Swim,
        /// <summary>
        /// Hike
        /// </summary>
        Hike,
        /// <summary>
        /// Walk
        /// </summary>
        Walk,
        /// <summary>
        /// AlpineSki
        /// </summary>
        AlpineSki,
        /// <summary>
        /// BackcountrySki
        /// </summary>
        BackcountrySki,
        /// <summary>
        /// Canoeing
        /// </summary>
        Canoeing,
        /// <summary>
        /// CrossCountrySkiing
        /// </summary>
        CrossCountrySkiing,
        /// <summary>
        /// Crossfit
        /// </summary>
        Crossfit,
        /// <summary>
        /// EBikeRide
        /// </summary>
        EBikeRide,
        /// <summary>
        /// Elliptical
        /// </summary>
        Elliptical,
        /// <summary>
        /// IceSkate
        /// </summary>
        IceSkate,
        /// <summary>
        /// InlineSkate
        /// </summary>
        InlineSkate,
        /// <summary>
        /// Kayaking
        /// </summary>
        Kayaking,
        /// <summary>
        /// Kitesurf
        /// </summary>
        Kitesurf,
        /// <summary>
        /// NordicSki
        /// </summary>
        NordicSki,
        /// <summary>
        /// RockClimbing
        /// </summary>
        RockClimbing,
        /// <summary>
        /// RollerSki
        /// </summary>
        RollerSki,
        /// <summary>
        /// Rowing
        /// </summary>
        Rowing,
        /// <summary>
        /// Snowboard
        /// </summary>
        Snowboard,
        /// <summary>
        /// Snowshoe
        /// </summary>
        Snowshoe,
        /// <summary>
        /// StairStepper
        /// </summary>
        StairStepper,
        /// <summary>
        /// StandUpPaddling
        /// </summary>
        StandUpPaddling,
        /// <summary>
        /// Surfing
        /// </summary>
        Surfing,
        /// <summary>
        /// VirtualRide
        /// </summary>
        VirtualRide,
        /// <summary>
        /// WeightTraining
        /// </summary>
        WeightTraining,
        /// <summary>
        /// Windsurf
        /// </summary>
        Windsurf,
        /// <summary>
        /// Workout
        /// </summary>
        Workout,
        /// <summary>
        /// Yoga
        /// </summary>
        Yoga
    }

    /// <summary>
    /// Used by the Club class.
    /// </summary>
    public enum SportType
    {
        /// <summary>
        /// The club is for cyclists.
        /// </summary>
        Cycling,
        /// <summary>
        /// The club is for runners.
        /// </summary>
        Running,
        /// <summary>
        /// The club is for triathletes.
        /// </summary>
        Triathlon,
        /// <summary>
        /// Other club.
        /// </summary>
        Other
    }

    /// <summary>
    /// This enum represents the category of a segment.
    /// </summary>
    public enum ClimbCategory
    {
        /// <summary>
        /// The segment is a HC climb.
        /// </summary>
        CategoryHc,
        /// <summary>
        /// The segment is a Cat 4 climb.
        /// </summary>
        Category4,
        /// <summary>
        /// The segment is a Cat 3 climb.
        /// </summary>
        Category3,
        /// <summary>
        /// The segment is a Cat 2 climb.
        /// </summary>
        Category2,
        /// <summary>
        /// The segment is a Cat 1 climb.
        /// </summary>
        Category1,
        /// <summary>
        /// The segment is not categorized (usually a very flat segment).
        /// </summary>
        CategoryNc
    }

    /// <summary>
    /// This enum is used by the Club class and represents the type of a club.
    /// </summary>
    public enum ClubType
    {
        /// <summary>
        /// The club is a casual club.
        /// </summary>
        Casual,
        /// <summary>
        /// The club is a racing team.
        /// </summary>
        RacingTeam,
        /// <summary>
        /// The club is owned by a shop.
        /// </summary>
        Shop,
        /// <summary>
        /// The club's members are all riding for a company.
        /// </summary>
        Company,
        /// <summary>
        /// Other club.
        /// </summary>
        Other
    }

    public enum ActivitySort
    {
        /// <summary>
        /// Order the activity according to the activity's start time
        /// Looks best on the UI as "xx hours ago"
        /// </summary>
        StartTime,

        /// <summary>
        /// Order the activity according to the activity's end time
        /// </summary>
        EndTime
    }
}
