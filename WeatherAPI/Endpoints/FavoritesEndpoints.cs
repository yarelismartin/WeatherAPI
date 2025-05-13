using WeatherAPI.Models;

namespace WeatherAPI.Endpoints
{
    public static class FavoritesEndpoints
    {
        public static void MapFavoriteEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("favorites").WithTags(nameof(Favorites));
        }
    }
}
