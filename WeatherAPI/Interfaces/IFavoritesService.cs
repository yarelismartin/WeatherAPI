using Microsoft.AspNetCore.Components.Routing;
using WeatherAPI.DTO;
using WeatherAPI.Models;

namespace WeatherAPI.Interfaces
{
    public interface IFavoritesService
    {
        Task<(bool Success, string Message)> AddFavoriteLocationAsync(int userId, int locationId);
        Task<(bool Success, string Message)> RemoveFavoriteLocationAsync(int userId, int locationId);
        Task<(bool Success, List<FavoritedLocationDTO>? UserFavorites, string? Message)> GetUsersFavoritesAsync(int userId);
    }
}
