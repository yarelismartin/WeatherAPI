using WeatherAPI.Models;

namespace WeatherAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> UserEmailInUseAsync(string userEmial);
        Task<User> CreateUserAsync(User User);
        Task<User?> GetByEmailAsync(string email);
        bool VerifyPassword(string storedPasswordHash, string providedPassword);
        Task<User?> GetUserByIdAsync(int userId);
    }
}
