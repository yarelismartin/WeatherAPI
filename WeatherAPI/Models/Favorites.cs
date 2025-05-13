namespace WeatherAPI.Models
{
    public class Favorites
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

    }
}
