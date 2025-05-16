using WeatherAPI.Models;

namespace WeatherAPI.DTO
{
    public class FavoritedLocationDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public FavoritedLocationDTO(Location location) 
        { 
            Id = location.Id;
            Name = location.Name;
        }

    }
}
