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
                    logger.LogWarning("Unauthorized request to get favorites — no valid user ID in token.");
                    return Results.Unauthorized();
                }

                var result = await favoritesService.GetUsersFavoritesAsync(parsedUserId);
                
                return result;
            });

            group.MapPost("/", async (HttpContext context, IFavoritesService favoriteService, ILogger < Program > logger, AddFavoriteDTO dto) =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                {
                    logger.LogWarning("Unauthorized request to add favorite — no valid user ID in token.");
                    return Results.Unauthorized();
                }

                try
                {
                    await favoriteService.AddFavoriteLocationAsync(parsedUserId, dto.LocationId);
                    logger.LogInformation("User {UserId} added location {LocationId} to favorites.", parsedUserId, dto.LocationId);
                    return Results.Ok(new { message = "Location added to favorites." });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to add favorite for user {UserId}", parsedUserId);
                    return Results.Problem("Failed to add favorite.");
                }
            });


            group.MapDelete("/{id}", async (HttpContext context, IFavoritesService favoriteService, ILogger<Program> logger, int id) =>
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int parsedUserId))
                {
                    logger.LogWarning("Unauthorized request to add favorite — no valid user ID in token.");
                    return Results.Unauthorized();
                }

                try
                {
                    await favoriteService.RemoveFavoriteLocationAsync(parsedUserId, id);
                    logger.LogInformation("User {UserId} removed location {LocationId} from favorites.", parsedUserId, id);
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to remove favorite for user {UserId}", parsedUserId);
                    return Results.Problem("Failed to remove favorite.");
                }
            });




        }
    }
}
