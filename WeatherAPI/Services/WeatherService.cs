using System.Net.Http;
using System.Text.Json;
using WeatherAPI.Interfaces;
using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "8636463fadf0e57cdc70806325688654";

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Call the Geocoding API in order to gather the lat and lon needed for the forecast api
        public async Task<GeocodingReturn> GetGeocodingDataAsync(string city)
        {
            var geoCodeURL = $"http://api.openweathermap.org/geo/1.0/direct?q={{city name}}&limit=1&appid={_apiKey}";
            var response = await _httpClient.GetStringAsync(geoCodeURL);
            var geocodingData = JsonSerializer.Deserialize<GeocodingReturn>(response);
            return geocodingData;
        }

        // call the forecast api and pass it the returned data from the GetGeocodingDataAsync method
        public async Task<WeatherData> GetWeatherDataAsync(string city)
        {
            var geocodingData = await GetGeocodingDataAsync(city);

            if (geocodingData == null)
            {
                //log a message
                return null;
            }

            var weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={geocodingData.Lat}&lon={geocodingData.Lon}&appid={_apiKey}";
            var response = await _httpClient.GetStringAsync(weatherUrl);
            var weatherData = JsonSerializer.Deserialize<WeatherData>(response);

            return weatherData;
        
        }
    }
}
