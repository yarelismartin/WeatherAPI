using WeatherAPI.Models;

namespace WeatherAPI.Data
{
    public class FavoritesData
    {
        public static List<Favorites> Favorites = new()
        {
            new Favorites { Id = 1, Location = "New York", UserId = 1 },
            new Favorites { Id = 2, Location = "London", UserId = 1 },
            new Favorites { Id = 3, Location = "Sydney", UserId = 2 },
            new Favorites { Id = 4, Location = "Paris", UserId = 2 },
            new Favorites { Id = 5, Location = "Tokyo", UserId = 3 },
            new Favorites { Id = 6, Location = "Berlin", UserId = 3 }
        };
    }
}
