using Microsoft.AspNetCore.Components.Routing;
using WeatherAPI.Models;

namespace WeatherAPI.Interfaces
{
    public interface IFavoritesService
    {
        Task<IResult> AddFavoriteLocationAsync(int userId, int locationId);
        Task<IResult> RemoveFavoriteLocationAsync(int userId, int locationId);
        Task<IResult> GetUsersFavoritesAsync(int userId);
    }
}
