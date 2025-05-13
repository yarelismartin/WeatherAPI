namespace WeatherAPI.Models
{
    public class DailyForecast
    {
        public long Dt { get; set; }
        public Temperature Temp { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
    }
}
