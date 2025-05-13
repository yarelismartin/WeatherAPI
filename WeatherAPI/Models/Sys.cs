using System.Text.Json.Serialization;

namespace WeatherAPI.Models
{
    public class Sys
    {
        [JsonPropertyName("sunrise")]
        public long Sunrise { get; set; }

        [JsonPropertyName("sunset")]
        public long Sunset { get; set; }
    }
}
