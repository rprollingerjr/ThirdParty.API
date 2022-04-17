using System;
using Newtonsoft.Json;

namespace ThirdParty.API.Models
{
    public class InstagramPostLocalModel
    {
        public InstagramPostLocalModel()
        {
        }

        [JsonProperty("id")]
        public long? ID { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("media_type")]
        public string MediaType { get; set; }

        [JsonProperty("media_url")]
        public string MediaURL { get; set; }

        [JsonProperty("timestamp")]
        public string PostDate { get; set; }

    }
}
