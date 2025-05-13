using WeatherAPI.Data;
using WeatherAPI.Interfaces;

namespace WeatherAPI.Repositories
{
    public class UserRespository : IUserRepository
    {
        private readonly WeatherAPIDbContext dbContext;

        public UserRespository(WeatherAPIDbContext context)
        {
            dbContext = context;
        }
    }
}
