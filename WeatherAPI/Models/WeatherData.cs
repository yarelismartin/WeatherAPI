namespace WeatherAPI.Models
{
    public class WeatherData
    {
        public CurrentWeather Current {  get; set; }
        public List<DailyForecast> Daily {  get; set; }
    }
}
