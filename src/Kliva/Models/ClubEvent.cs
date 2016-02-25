using Newtonsoft.Json;

namespace Kliva.Models
{
    public class ClubEvent
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("resource_state")]
        public int ResourceState { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("club_id")]
        public int ClubId { get; set; }

        [JsonProperty("organizing_athlete")]
        public AthleteSummary Organizer { get; set; }

        [JsonProperty("activity_type")]
        public string ActivityType { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("route_id")]
        public int? RouteId { get; set; }

        [JsonProperty("woman_only")]
        public bool IsWomanOnly { get; set; }

        [JsonProperty("private")]
        public bool IsPrivate { get; set; }

        [JsonProperty("skill_levels")]
        public int SkillLevels { get; set; }

        [JsonProperty("terrain")]
        public int Terrain { get; set; }

        [JsonProperty("upcoming_occurences")]
        public string[] Occurences { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
