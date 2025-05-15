using Microsoft.EntityFrameworkCore;
using WeatherAPI.Data;
using WeatherAPI.Interfaces;
using WeatherAPI.Models;

namespace WeatherAPI.Repositories
{
    public class FavoritesRepository : IFavoritesRepository
    {
        private readonly WeatherAPIDbContext dbContext;

        public FavoritesRepository(WeatherAPIDbContext context)
        {
            dbContext = context;
        }

        public async Task<User> GetFavoritesByUserId(int userId)
        {
            return await dbContext.Users
                .Include(u => u.FavoriteLocations)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> AddFavoriteAsync(int userId, int locationId)
        {
            var user = await dbContext.Users
                .Include(u => u.FavoriteLocations)
                .SingleOrDefaultAsync(u => u.Id == userId);

            var location = await dbContext.Locations
                .SingleOrDefaultAsync(f => f.Id == locationId);

            user.FavoriteLocations?.Add(location);
            await dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> RemoveFavoriteAsync(int userId, int locationId)
        {
            var user = await dbContext.Users
                .Include(u => u.FavoriteLocations)
                .SingleOrDefaultAsync(u => u.Id == userId);

            var location = await dbContext.Locations
                .SingleOrDefaultAsync(f => f.Id == locationId);

            user.FavoriteLocations?.Remove(location);
            await dbContext.SaveChangesAsync();

            return user;
        }
    }
}
