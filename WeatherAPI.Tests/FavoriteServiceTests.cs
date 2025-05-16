using Moq;
using WeatherAPI.Interfaces;
using WeatherAPI.Models;
using WeatherAPI.Services;

namespace WeatherAPI.Tests
{
    public class FavoriteServiceTests
    {
        private readonly Mock<IFavoritesRepository> _mockFavoritesRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly FavoritesService _service;

        public FavoritesServiceTests()
        {
            _mockFavoritesRepo = new Mock<IFavoritesRepository>();
            _mockUserRepo = new Mock<IUserRepository>();

            _service = new FavoritesService(_mockFavoritesRepo.Object, _mockUserRepo.Object);
        }

        [Fact]
        public async Task GetUsersFavoritesAsync_ShouldReturnFavorites_WhenUserHasFavorites()
        {
            // Arrange
            int userId = 1;
            var favoriteLocations = new List<Location>
        {
            new Location { Id = 101, Name = "New York" },
            new Location { Id = 102, Name = "Los Angeles" }
        };

            var userWithFavorites = new User
            {
                Id = userId,
                FavoriteLocations = favoriteLocations
            };

            _mockFavoritesRepo
                .Setup(repo => repo.GetFavoritesByUserId(userId))
                .ReturnsAsync(userWithFavorites);

            // Act
            var result = await _service.GetUsersFavoritesAsync(userId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.UserFavorites);
            Assert.Equal(2, result.UserFavorites.Count);
            Assert.Equal("Successfully retrieved favorite locations", result.Message);
        }


        [Fact]
        public async Task AddFavoriteLocationAsync_ShouldAddFavorite_WhenUserExistsAndLocationNotFavorited()
        {
            // Arrange
            int userId = 1;
            int locationId = 100;

            var user = new User
            {
                Id = userId,
                FavoriteLocations = new List<Location>() // user has no favorites
            };

            var mockUser = new User
            {
                Id = userId,
                Email = "test@example.com",
                FavoriteLocations = new List<Location>
    {
        new Location { Id = 1, Name = "Test Location" },
        new Location { Id = 2, Name = "Another Location" }
    }
            };

            var mockFavoritesRepo = new Mock<IFavoritesRepository>();
            var mockUserRepo = new Mock<IUserRepository>();

            mockUserRepo
                .Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            mockFavoritesRepo
                .Setup(repo => repo.AddFavoriteAsync(userId, locationId))
                .ReturnsAsync(mockUser);

            var service = new FavoritesService(mockFavoritesRepo.Object, mockUserRepo.Object);

            // Act
            var result = await service.AddFavoriteLocationAsync(userId, locationId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Location added to favorites.", result.Message);

            // Also confirm AddFavoriteAsync was called
            mockFavoritesRepo.Verify(repo => repo.AddFavoriteAsync(userId, locationId), Times.Once);
        }


        [Fact]
        public async Task RemoveFavoriteLocationAsync_ShouldRemoveFavorite_WhenUserExistsAndLocationIsFavorited()
        {
            // Arrange
            int userId = 1;
            int locationId = 100;

            var user = new User
            {
                Id = userId,
                FavoriteLocations = new List<Location>
        {
            new Location { Id = locationId } 
        }
            };

            var mockUser = new User
            {
                Id = userId,
                Email = "test@example.com",
                FavoriteLocations = new List<Location>
    {
        new Location { Id = 1, Name = "Test Location" },
        new Location { Id = 2, Name = "Another Location" }
    }
            };


            var mockFavoritesRepo = new Mock<IFavoritesRepository>();
            var mockUserRepo = new Mock<IUserRepository>();

            mockUserRepo
                .Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            mockFavoritesRepo
                .Setup(repo => repo.RemoveFavoriteAsync(userId, locationId))
                .ReturnsAsync(mockUser);

            var service = new FavoritesService(mockFavoritesRepo.Object, mockUserRepo.Object);

            // Act
            var result = await service.RemoveFavoriteLocationAsync(userId, locationId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Successfully removed location from favorites", result.Message);

            mockFavoritesRepo.Verify(repo => repo.RemoveFavoriteAsync(userId, locationId), Times.Once);
        }

    }
}