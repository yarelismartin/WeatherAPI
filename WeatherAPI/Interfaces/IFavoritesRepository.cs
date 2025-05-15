using WeatherAPI.Models;

namespace WeatherAPI.Interfaces
{
    public interface IFavoritesRepository
    {
        Task<User> GetFavoritesByUserId(int userId);

        Task<User> AddFavoriteAsync(int userId, int locationId);

        Task<User> RemoveFavoriteAsync(int userId, int locationId);

    }
}
