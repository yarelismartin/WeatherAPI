using WeatherAPI.Interfaces;

namespace WeatherAPI.Services
{
    public class FavoritesService : IFavoritesService
    {
        private readonly IFavoritesRepository _favoritesRepository;
        
        public FavoritesService(IFavoritesRepository favoritesRepository)
        {
            _favoritesRepository = favoritesRepository;
        }
    }
}
