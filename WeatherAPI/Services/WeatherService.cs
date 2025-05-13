using System.Net.Http;
using WeatherAPI.Interfaces;

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

        //public async Task<WeatherForecast>
    }
}
