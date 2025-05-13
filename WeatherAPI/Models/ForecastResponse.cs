using System.Text.Json.Serialization;

namespace WeatherAPI.Models
{
    public class ForecastResponse
    {
        [JsonPropertyName("cod")]
        public string Cod { get; set; }

        [JsonPropertyName("message")]
        public int Message { get; set; }

        [JsonPropertyName("cnt")]
        public int Cnt { get; set; }

        [JsonPropertyName("list")]
        public List<ForecastItem> List { get; set; }

        [JsonPropertyName("city")]
        public City City { get; set; }
    }
}
