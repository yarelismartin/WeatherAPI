using WeatherAPI.Models;

namespace WeatherAPI.Data
{
    public class UserData
    {
        public static List<User> Users = new()
        {
            new User
            {
                Id = 1,
                Username = "john_doe",
                PasswordHash = "hashedPassword123",
            },
            new User
            {
                Id = 2,
                Username = "jane_smith",
                PasswordHash = "hashedPassword456",
            },
            new User
            {
                Id = 3,
                Username = "alex_jones",
                PasswordHash = "hashedPassword789",
            }
        };
    }
}
