namespace Kliva.Models
{
    public enum DistanceUnitType
    {
        Kilometres,
        Miles,
        Metres,
        Feet
    }

    public enum DisplayMode
    {
        CompactOverlay,
        Inline
    }

    public enum MenuItemType
    {
        MDL2,
        Material
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
}
