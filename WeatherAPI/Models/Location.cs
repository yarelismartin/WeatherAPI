﻿namespace WeatherAPI.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> FavoritedByUsers { get; set; } = new();

    }
}
