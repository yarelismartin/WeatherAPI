using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using WeatherAPI.Interfaces;
using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<WeatherService> _logger;
        private readonly IMemoryCache _cache;

        public WeatherService(HttpClient httpClient, ILogger<WeatherService> logger, IMemoryCache cache, IConfiguration config)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
            _apiKey = config["OpenWeatherMap:ApiKey"];

        }

        // Call the Geocoding API in order to gather the lat and lon needed for the forecast api
        public async Task<(double lat, double lon)?> GetGeocodingDataAsync(string city)
        {
            // construct a cache key that will identify the geocoding for a city
            var cacheKey = $"geo_{city.ToLower()}";

            // calling the IMemoryCache thta holds the catch data and trying to get the value of they cachekey above to then assign its value to the cachedCoordinates
            if (_cache.TryGetValue(cacheKey, out (double lat, double lon)? cachedCoordinates))
            {
                _logger.LogInformation("Returning cached geocoding data for {City}", city);

                // return the catchedCoordinates that holds correct values for this cacheKey
                return cachedCoordinates;
            }

            try
            {
                var geoCodeURL = $"http://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={_apiKey}";
                var response = await _httpClient.GetStringAsync(geoCodeURL);
                _logger.LogInformation($"Geocoding API Response: {response}");

                using var document = JsonDocument.Parse(response);
                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var location = root[0];
                    double lat = location.GetProperty("lat").GetDouble();
                    double lon = location.GetProperty("lon").GetDouble();

                    // This line adds or updates an entry in the cache with the specified key, value, and expiration time
                    _cache.Set(cacheKey, (lat, lon), TimeSpan.FromMinutes(30));

                    return (lat, lon);
                }

                _logger.LogWarning("No geocoding data found for city: {City}", city);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetGeocodingDataAsync)} for city: {city}");
                return null;
            }
        }

        // call the get current weather api and pass it the returned data from the GetGeocodingDataAsync method
        public async Task<WeatherData> GetWeatherDataAsync(string city)
        {

            var cacheKey = $"current_weather_{city.ToLower()}";

            if (_cache.TryGetValue(cacheKey, out WeatherData cachedWeather))
            {
                _logger.LogInformation("Returning cached current weather for {City}", city);
                return cachedWeather;
            }

            try
            {
                var coordinates = await GetGeocodingDataAsync(city);

                if (coordinates == null)
                {
                    _logger.LogWarning("Cannot retrieve weather: No coordinates found for city: {City}", city);
                    return null;
                }

                var (lat, lon) = coordinates.Value;

                if (lat == 0 || lon == 0)
                {
                    _logger.LogWarning("Coordinates returned as 0,0 for city: {City}", city);
                    return null;
                }

                var weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}";
                var response = await _httpClient.GetStringAsync(weatherUrl);
                _logger.LogInformation($"Weather API Response: {response}");

                var weatherData = JsonSerializer.Deserialize<WeatherData>(response);

                // Cache the result for 10 minutes
                _cache.Set(cacheKey, weatherData, TimeSpan.FromMinutes(10));

                return weatherData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetWeatherDataAsync)} for city: {city}");
                return null;
            }
        }

        // call the forecast api to display the forecast 5 days from now
        public async Task<ForecastResponse> GetFiveDayForecastAsync(string city)
        {
            var cacheKey = $"forecast_{city.ToLower()}";

            if (_cache.TryGetValue(cacheKey, out ForecastResponse cachedForecast))
            {
                _logger.LogInformation("Returning cached forecast for {City}", city);
                return cachedForecast;
            }

            try
            {
                var coordinates = await GetGeocodingDataAsync(city);

                if (coordinates == null)
                {
                    _logger.LogWarning("Cannot retrieve weather: No coordinates found for city: {City}", city);
                    return null;
                }

                var (lat, lon) = coordinates.Value;

                if (lat == 0 || lon == 0)
                {
                    _logger.LogWarning("Coordinates returned as 0,0 for city: {City}", city);
                    return null;
                }

                var forecastUrl = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={_apiKey}";
                var response = await _httpClient.GetStringAsync(forecastUrl);
                _logger.LogInformation($"Forecast API Response: {response}");

                var forecastData = JsonSerializer.Deserialize<ForecastResponse>(response);

                // Cache the result for 30 minutes
                _cache.Set(cacheKey, forecastData, TimeSpan.FromMinutes(30));


                return forecastData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetFiveDayForecastAsync)} for city: {city}");
                return null;
            }
        }
    }
}
