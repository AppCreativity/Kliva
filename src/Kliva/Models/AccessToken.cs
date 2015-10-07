using Newtonsoft.Json;
using System;

namespace Kliva.Models
{
    public class AccessToken
    {
        [JsonProperty("access_token")]
        public String Token { get; set; }
    }
}
