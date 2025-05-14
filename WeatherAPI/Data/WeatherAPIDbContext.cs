using Microsoft.EntityFrameworkCore;
using WeatherAPI.Models;

namespace WeatherAPI.Data
{
    public class WeatherAPIDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Favorites> Favorites { get; set; }

        public WeatherAPIDbContext(DbContextOptions<WeatherAPIDbContext> context) : base(context) 
        { 
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(UserData.Users);
            modelBuilder.Entity<Favorites>().HasData(FavoritesData.Favorites);
            
        }


    }
}
