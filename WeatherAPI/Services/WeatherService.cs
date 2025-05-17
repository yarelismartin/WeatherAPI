using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using WeatherAPI.Interfaces;
using WeatherAPI.Models;
using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;

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
        public async Task<(bool success, (double lat, double lon)? coordinates, string message)> GetGeocodingDataAsync(string city)
        {
            // construct a cache key that will identify the geocoding for a city
            var cacheKey = $"geo_{city.ToLower()}";

            // calling the IMemoryCache thta holds the catch data and trying to get the value of they cachekey above to then assign its value to the cachedCoordinates
            if (_cache.TryGetValue(cacheKey, out (double lat, double lon)? cachedCoordinates))
            {
                Log.Information("Returning cached geocoding data for {City}", city);

                // return the catchedCoordinates that holds correct values for this cacheKey
                return (true, cachedCoordinates, "Success (from cache).");
            }

            try
            {
                var geoCodeURL = $"http://api.openweathermap.org/geo/1.0/direct?q={city}&limit=1&appid={_apiKey}";
                var response = await _httpClient.GetStringAsync(geoCodeURL);
                Log.Information($"Geocoding API Response: {response}");

                using var document = JsonDocument.Parse(response);
                var root = document.RootElement;

                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var location = root[0];
                    double lat = location.GetProperty("lat").GetDouble();
                    double lon = location.GetProperty("lon").GetDouble();

                    // This line adds or updates an entry in the cache with the specified key, value, and expiration time
                    _cache.Set(cacheKey, (lat, lon), TimeSpan.FromMinutes(30));

                    Log.Information("Caching geocoding data for {City}: lat={Lat}, lon={Lon}", city, lat, lon);
                    return (true, (lat, lon), "Success");
                }

                Log.Warning("No geocoding data found for city: {City}", city);
                return (false, null, "No location data found for this city.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving geocoding data for city: { City}", city);
                throw;
            }
        }

        // call the get current weather api and pass it the returned data from the GetGeocodingDataAsync method
        public async Task<(bool success, WeatherData? data, string message)> GetWeatherDataAsync(string city)
        {

            if (string.IsNullOrWhiteSpace(city))
            {
                Log.Warning("City input is null or whitespace.");
                return (false, null, "City name cannot be empty.");
            }

            var cacheKey = $"current_weather_{city.ToLower()}";

            if (_cache.TryGetValue(cacheKey, out WeatherData cachedWeather))
            {
                Log.Information("Returning cached current weather for {City}", city);
                return (true, cachedWeather, "Success (from cache).");
            }

            try
            {
                var (success, coordinates, message) = await GetGeocodingDataAsync(city);

                if (!success || coordinates == null)
                {
                    Log.Warning("Cannot retrieve forecast: {Message}", message);
                    return (false, null, message);
                }

                var (lat, lon) = coordinates.Value;

                if (lat == 0 || lon == 0)
                {
                    Log.Warning("Coordinates returned as 0,0 for city: {City}", city);
                    return (false, null, "Invalid coordinates received for city.");
                }

                var weatherUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}";
                var response = await _httpClient.GetStringAsync(weatherUrl);
                Log.Information("Weather API Response: {Weather}", response);

                var weatherData = JsonSerializer.Deserialize<WeatherData>(response);

                if (weatherData == null)
                {
                    Log.Warning("Failed to deserialize weather data for city: {City}", city);
                    return (false, null, "Failed to parse weather data.");
                }

                // Cache the result for 10 minutes
                _cache.Set(cacheKey, weatherData, TimeSpan.FromMinutes(10));

                return (true, weatherData, "Success");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error in {nameof(GetWeatherDataAsync)} for city: {city}");
                throw;
            }
        }

        // call the forecast api to display the forecast 5 days from now
        public async Task<(bool success, ForecastResponse? data, string message)> GetFiveDayForecastAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                Log.Warning("City input is null or whitespace.");
                return (false, null, "City name cannot be empty.");
            }

            var cacheKey = $"forecast_{city.ToLower()}";

            if (_cache.TryGetValue(cacheKey, out ForecastResponse cachedForecast))
            {
                Log.Information("Returning cached forecast for {City}", city);
                return (true, cachedForecast, $"Returning cached forecast for {city}.");
            }

            try
            {
                var (success, coordinates, message) = await GetGeocodingDataAsync(city);

                if (!success || coordinates == null)
                {
                    Log.Warning("Cannot retrieve forecast: {Message}", message);
                    return (false, null, message);
                }

                var (lat, lon) = coordinates.Value;

                if (lat == 0 || lon == 0)
                {
                    Log.Warning("Coordinates returned as 0,0 for city: {City}", city);
                    return (false, null, $"Invalid coordinates (0,0) for {city}.");
                }

                var forecastUrl = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={_apiKey}";
                var response = await _httpClient.GetStringAsync(forecastUrl);
                _logger.LogInformation($"Forecast API Response: {response}");

                var forecastData = JsonSerializer.Deserialize<ForecastResponse>(response);

                if (forecastData == null)
                {
                    Log.Warning("Forecast data could not be deserialized for city: {City}", city);
                    return (false, null, $"Failed to parse forecast data for {city}.");
                }

                // Cache the result for 30 minutes
                Log.Information("Caching forecast data for {City}", city);
                _cache.Set(cacheKey, forecastData, TimeSpan.FromMinutes(30));


                return (true, forecastData, $"Successfully retrieved 5-day forecast for {city}.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in {Method} for city: {City}", nameof(GetFiveDayForecastAsync), city);
                throw;
            }
        }
    }
}
