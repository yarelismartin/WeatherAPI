using WeatherAPI.Models;

namespace WeatherAPI.Data
{
    public class LocationData
    {
        public static List<Location> Locations = new()
        {
           new Location { Id = 1, Name = "New York City" },
        new Location { Id = 2, Name = "Los Angeles" },
        new Location { Id = 3, Name = "Chicago" },
        new Location { Id = 4, Name = "Houston" },
        new Location { Id = 5, Name = "Miami" }
        };
    }
}
