using Newtonsoft.Json;
using System;

namespace Kliva.Models
{
    public class AccessToken
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }
    }
}
