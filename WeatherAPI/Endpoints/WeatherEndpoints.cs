using Serilog;
using WeatherAPI.Interfaces;
using WeatherAPI.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WeatherAPI.Endpoints
{
    public static class WeatherEndpoints
    {
        public static void MapWeatherEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("api/weather").WithTags("Weather");

            group.MapGet("/{location}", async (IWeatherService weatherService, string location) =>
            {

                try
                {
                    var (success, data, message) = await weatherService.GetWeatherDataAsync(location);

                    if (!success || data == null)
                    {
                        Log.Warning("Weather data not found for the following location: {Location}", location);
                        return Results.NotFound(message);
                    }

                    Log.Information("Current Weather fetched successfully: {FavoritedLocations}", data);
                    return Results.Ok(data);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occured while fetching current weather for {City}", location);
                    return Results.Problem("An unexpected error occurred. Please try again later.");
                }

            });

            group.MapGet("/forcast/{location}", async (IWeatherService weatherService, string location) =>
            {
                try
                {
                    var (success, data, message) = await weatherService.GetFiveDayForecastAsync(location);
                    
                    if (!success || data == null)
                    {
                        Log.Warning("Forecast data not found for the following location: {Location}", location);
                        return Results.NotFound(message);
                    }

                    Log.Information("Current Weather fetched successfully: {FavoritedLocations}", data);
                    return Results.Ok(data);

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occured while fetching current weather for {City}", location);
                    return Results.Problem("An unexpected error occurred. Please try again later.");
                }
                
            });
        }
    }
}
