using Newtonsoft.Json;

namespace Kliva.Models
{
    public class AccessToken
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }
    }
}
