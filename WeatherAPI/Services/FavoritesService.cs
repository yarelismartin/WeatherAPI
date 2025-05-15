using WeatherAPI.Interfaces;
using WeatherAPI.Models;
using WeatherAPI.Repositories;

namespace WeatherAPI.Services
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IFavoritesRepository _favoritesRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJWTToken _jWTToken;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<FavoritesService> _logger;


        public FavoritesService(IFavoritesRepository favoritesRepository, IJWTToken jWTToken, IHttpContextAccessor httpContextAccessor, ILogger<FavoritesService> logger, IUserRepository userRepository)
        {
            _favoritesRepository = favoritesRepository;
            _userRepository = userRepository;
            _jWTToken = jWTToken;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IResult> AddFavoriteLocationAsync(int userId, int locationId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return Results.NotFound("User not found.");
                }

                if (user.FavoriteLocations.Any(f => f.Id == locationId))
                {
                    _logger.LogInformation("User {UserId} already has location {LocationId} as a favorite.", userId, locationId);
                    return Results.Conflict("Location is already in favorites.");
                }


                await _favoritesRepository.AddFavoriteAsync(userId, locationId);

                _logger.LogInformation("Added location {LocationId} to favorites for user {UserId}.", locationId, userId);
                return Results.Ok("Location added to favorites.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding favorite for user {UserId}", userId);
                return Results.Problem("An error occurred while adding the favorite.");
            }

        }

        public async Task<IResult> RemoveFavoriteLocationAsync(int userId, int locationId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", userId);
                    return Results.NotFound("User not found.");
                }

                if (!user.FavoriteLocations.Any(f => f.Id == locationId))
                {
                    _logger.LogInformation("User {UserId} does not have {LocationId} as a favorite.", userId, locationId);
                    return Results.Conflict("Location is not in favorites.");
                }


                await _favoritesRepository.RemoveFavoriteAsync(userId, locationId);

                _logger.LogInformation("Removed location {LocationId} from favorites for user {UserId}.", locationId, userId);
                return Results.NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while removing kication for user {UserId} favorites", userId);
                return Results.Problem("An error occurred while removing the favorite.");
            }

        }

        public async Task<IResult> GetUsersFavoritesAsync(int userId)
        {

            if(userId == null) 
            {
                _logger.LogWarning("Unauthorized request to get favorites — no user ID in token.");
                return Results.Unauthorized();
            }

            _logger.LogInformation("Fetching favorites for user {UserId}", userId);

            // fetch the users favorites using the repo layer
            var favorites = await _favoritesRepository.GetFavoritesByUserId(userId);
            if (!favorites.FavoriteLocations.Any())
            {
                _logger.LogInformation("No favorites found for user {UserId}", userId);
                return Results.NotFound("No favorite locations found.");
            }

            //return ok
            _logger.LogInformation("Found {Count} favorite(s) for user {UserId}", favorites.FavoriteLocations.Count(), userId);
            return Results.Ok(favorites);
        }

    }
}
