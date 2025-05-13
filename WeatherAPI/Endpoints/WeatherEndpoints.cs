using WeatherAPI.Interfaces;
using WeatherAPI.Models;

namespace WeatherAPI.Endpoints
{
    public static class WeatherEndpoints
    {
        public static void MapWeatherEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("api/weather").WithTags("Weather");

            group.MapGet("/{location}", async (IWeatherService weatherService, string location) =>
            {
                var currentWeather = await weatherService.GetWeatherDataAsync(location);

                if (currentWeather == null)
                {
                    return Results.NotFound($"Weather data not found for the following location: {location}");
                }

                return Results.Ok(currentWeather);
            });
        }
    }
}
