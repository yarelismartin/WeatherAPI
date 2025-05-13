using WeatherAPI.Data;
using WeatherAPI.Interfaces;

namespace WeatherAPI.Repositories
{
    public class FavoritesRepository : IFavoritesRepository
    {
        private readonly WeatherAPIDbContext dbContext;

        public FavoritesRepository(WeatherAPIDbContext context)
        {
            dbContext = context;
        }
    }
}
