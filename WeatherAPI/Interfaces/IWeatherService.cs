using WeatherAPI.Models;

namespace WeatherAPI.Interfaces
{
    public interface IWeatherService
    {
        Task<GeocodingReturn> GetGeocodingDataAsync(string city);
        Task<WeatherData> GetWeatherDataAsync(string city);
    }
}
