using WeatherAPI.Data;
using WeatherAPI.Interfaces;

namespace WeatherAPI.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly WeatherAPIDbContext dbContext;

        public WeatherRepository(WeatherAPIDbContext context)
        {
            dbContext = context;
        }
    }
}
