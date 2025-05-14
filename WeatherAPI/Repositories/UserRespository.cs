using Microsoft.EntityFrameworkCore;
using System.Text;
using WeatherAPI.Data;
using WeatherAPI.Interfaces;
using WeatherAPI.Models;

namespace WeatherAPI.Repositories
{
    public class UserRespository : IUserRepository
    {
        private readonly WeatherAPIDbContext dbContext;

        public UserRespository(WeatherAPIDbContext context)
        {
            dbContext = context;
        }

        public async Task<bool> UserEmailInUseAsync(string userEmial)
        {
            return await dbContext.Users.AnyAsync(u => u.Email == userEmial);
        }

        public async Task<User> CreateUserAsync(User User)
        {
            await dbContext.Users.AddAsync(User);
            await dbContext.SaveChangesAsync();
            return User;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public bool VerifyPassword(string storedPasswordHash, string providedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(providedPassword, storedPasswordHash);
        }

    }
}
