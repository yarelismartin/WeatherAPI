using System.Text.Json.Serialization;

namespace WeatherAPI.Models
{
    public class Clouds
    {
        [JsonPropertyName("all")]
        public int All { get; set; }
    }
}
