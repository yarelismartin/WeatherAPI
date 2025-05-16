using WeatherAPI.Models;

namespace WeatherAPI.Interfaces
{
    public interface IWeatherService
    {
        Task<(bool success, (double lat, double lon)? coordinates, string message)> GetGeocodingDataAsync(string city);
        Task<(bool success, WeatherData? data, string message)> GetWeatherDataAsync(string city);
        Task<(bool success, ForecastResponse? data, string message)> GetFiveDayForecastAsync(string city);
    }
}
