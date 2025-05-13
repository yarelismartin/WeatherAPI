using WeatherAPI.Models;

namespace WeatherAPI.Interfaces
{
    public interface IWeatherService
    {
        Task<(double lat, double lon)?> GetGeocodingDataAsync(string city);
        Task<WeatherData> GetWeatherDataAsync(string city);
        Task<ForecastResponse> GetFiveDayForecastAsync(string city);
    }
}
