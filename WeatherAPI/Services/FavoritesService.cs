using WeatherAPI.Interfaces;
using WeatherAPI.Models;
using WeatherAPI.Repositories;
using Serilog;
using WeatherAPI.DTO;

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

        public async Task<(bool Success, string Message)> AddFavoriteLocationAsync(int userId, int locationId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                Log.Warning("User with ID {UserId} not found.", userId);
                return (false, "User not found.");
            }

            if (user.FavoriteLocations.Any(f => f.Id == locationId))
            {
                Log.Information("User {UserId} already has location {LocationId} as a favorite.", userId, locationId);
                return (false, "Location is already in favorites.");
            }

            try
            {
                await _favoritesRepository.AddFavoriteAsync(userId, locationId);

                Log.Information("Added location {LocationId} to favorites for user {UserId}.", locationId, userId);
                
                return (true, "Location added to favorites.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while adding favorite location for user {UserId}", userId);
                throw;
            }

        }

        public async Task<(bool Success, string Message)> RemoveFavoriteLocationAsync(int userId, int locationId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                Log.Warning("User with ID {UserId} not found.", userId);
                return (false, "User not found.");
            }

            if (!user.FavoriteLocations.Any(f => f.Id == locationId))
            {
                Log.Information("User {UserId} does not have {LocationId} as a favorite.", userId, locationId);
                return (false, "The location you are trying to remove is not in favorites.");
            }
            try
            {


                await _favoritesRepository.RemoveFavoriteAsync(userId, locationId);

                Log.Information("Removed location {LocationId} from favorites for user {UserId}.", locationId, userId);
                return (true, "Successfully removed location from favorites");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while removing location {LocationId} from user {UserId} favorited", locationId, userId);
                throw;
            }

        }

        public async Task<(bool Success, List<FavoritedLocationDTO>? UserFavorites, string? Message)> GetUsersFavoritesAsync(int userId)
        {
            try
            {
                var favorites = await _favoritesRepository.GetFavoritesByUserId(userId);

                if (!favorites.FavoriteLocations.Any())
                {
                    Log.Information("No favorites found for user {UserId}", userId);
                    return (false, null, "No favorite locations found.");
                }

                Log.Information("Found {Count} favorite(s) for user {UserId}", favorites.FavoriteLocations.Count(), userId);

                var favoritedLocations = favorites.FavoriteLocations.Select(l => new FavoritedLocationDTO(l)).ToList();

                return (true, favoritedLocations, "Successfully retrieved favorite locations");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching favorites for user {UserId}", userId);
                throw; 
            }
        }

    }
}
