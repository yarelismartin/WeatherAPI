using Microsoft.Extensions.Logging;
using System.Security.Claims;
using WeatherAPI.DTO;
using WeatherAPI.Interfaces;
using WeatherAPI.Models;
using Serilog;

namespace WeatherAPI.Endpoints
{
    public static class FavoritesEndpoints
    {
        public static void MapFavoriteEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("api/favorites").WithTags("Favorites");

            group.MapGet("/", async (HttpContext context, IFavoritesService favoritesService, ILogger < Program > logger) =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                {
                    Log.Warning("Unauthorized request to get favorites — no valid user ID in token.");
                    return Results.Unauthorized();
                }

                try
                {

                    var (success, userFavorites,  message) = await favoritesService.GetUsersFavoritesAsync(parsedUserId);

                    if (!success)
                    {
                        Log.Warning("Fetching Favorites failed: {Message}", message);
                        return Results.NotFound(message);
                    }

                    Log.Information("User favorited fetched successfully: {FavoritedLocations}", userFavorites);
                    return Results.Ok(userFavorites);

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occured while fetching users favorited location");
                    return Results.Problem("An unexpected error occurred. Please try again later.");
                }
            });

            group.MapPost("/", async (HttpContext context, IFavoritesService favoriteService, ILogger < Program > logger, AddFavoriteDTO dto) =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                {
                    Log.Warning("Unauthorized request to add favorite — no valid user ID in token.");
                    return Results.Unauthorized();
                }

                try
                {
                    var (success, message) = await favoriteService.AddFavoriteLocationAsync(parsedUserId, dto.LocationId);
                    if (!success)
                    {
                        Log.Warning("Add favorite failed: {Message}", message);
                        return Results.BadRequest(message);
                    }

                    Log.Information("User {UserId} added location {LocationId} to favorites.", userId, dto.LocationId);
                    return Results.Ok(new { message });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to add favorite for user {UserId}", userId);
                    return Results.Problem("An unexpected error occurred while adding the favorite.");
                }
            });


            group.MapDelete("/{id}", async (HttpContext context, IFavoritesService favoriteService, ILogger<Program> logger, int id) =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                {
                    Log.Warning("Unauthorized request to add favorite — no valid user ID in token.");
                    return Results.Unauthorized();
                }

                try
                {
                    var (success, message) = await favoriteService.RemoveFavoriteLocationAsync(parsedUserId, id);
                    if (!success)
                    {
                        Log.Warning("Remove favorite failed: {Message}", message);
                        return Results.BadRequest(message);
                    }

                    Log.Information("User {UserId} removed location {LocationId} from favorites.", userId, id);
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to remove favorite for user {UserId}", parsedUserId);
                    return Results.Problem("Failed to remove favorite.");
                }
            });




        }
    }
}
