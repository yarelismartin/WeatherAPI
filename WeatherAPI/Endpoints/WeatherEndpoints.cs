using WeatherAPI.Models;

namespace WeatherAPI.Endpoints
{
    public static class WeatherEndpoints
    {
        public static void MapWeatherEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("weather").WithTags("Weather");
        }
    }
}
