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
                Email = "john_doe@gmail.com",
                PasswordHash = "hashedPassword123",
            },
            new User
            {
                Id = 2,
                Username = "jane_smith",
                Email = "jane_smith@gmail.com",
                PasswordHash = "hashedPassword456",
            },
            new User
            {
                Id = 3,
                Username = "alex_jones",
                Email = "alex_jones@gmail.com",
                PasswordHash = "hashedPassword789",
            }
        };
    }
}
