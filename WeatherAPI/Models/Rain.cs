using System.Text.Json.Serialization;

namespace WeatherAPI.Models
{
    public class Rain
    {
            [JsonPropertyName("3h")]
            public double Volume { get; set; }

    }
}
