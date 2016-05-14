using System.Collections.Generic;
using Newtonsoft.Json;

namespace Kliva.Models
{
    /// <summary>
    /// Represents the leaderboard of a segment.
    /// </summary>
    public class Leaderboard
    {
        /// <summary>
        /// The number of efforts.
        /// </summary>
        [JsonProperty("effort_count")]
        public int EffortCount { get; set; }

        /// <summary>
        /// The number of entries in the leaderboard.
        /// </summary>
        [JsonProperty("entry_count")]
        public int EntryCount { get; set; }

        /// <summary>
        /// A list of the entries of the leaderboard.
        /// </summary>
        [JsonProperty("entries")]
        public List<LeaderboardEntry> Entries { get; set; }

        /// <summary>
        /// Initializes a new instance of the Leaderboard class.
        /// </summary>
        public Leaderboard()
        {
            Entries = new List<LeaderboardEntry>();
        }
    }
}
