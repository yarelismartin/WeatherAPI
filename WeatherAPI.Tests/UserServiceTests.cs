using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using WeatherAPI.Services;
using WeatherAPI.Repositories;
using WeatherAPI.Interfaces;
using WeatherAPI.Utils;
using FluentAssertions;
using WeatherAPI.DTO;
using WeatherAPI.Models;
using Microsoft.Extensions.Configuration;

namespace WeatherAPI.Tests
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly JWTToken _jwtToken;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["ApplicationSettings:JWT_Secret"]).Returns("MySuperSecretKey1234567890123456");

            _jwtToken = new JWTToken(mockConfig.Object);
            _userService = new UserService(_mockUserRepository.Object, _jwtToken);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnTrue_WhenEmailIsNotInUse()
        {
            //Arrange 
            //sending server
            var dto = new RegisterUserDTO
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "password123"
            };
            
            _mockUserRepository
                .Setup(repo => repo.UserEmailInUseAsync(dto.Email))
                .ReturnsAsync(false);

            _mockUserRepository
               .Setup(repo => repo.CreateUserAsync(It.IsAny<User>()))
               .ReturnsAsync((User u) => u);

            //Act 
            var result = await _userService.RegisterAsync(dto);

            //Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("User registered successfully.");
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnSuccessAndToken_WhenCredentialsAreValid()
        {
            // Arrange
            var dto = new LoginUserDTO
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var user = new User
            {
                Id = 2,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password) // hash password to simulate real user
            };

            _mockUserRepository
                .Setup(repo => repo.UserEmailInUseAsync(dto.Email))
                .ReturnsAsync(true);

            _mockUserRepository
                .Setup(repo => repo.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            _mockUserRepository
                .Setup(repo => repo.VerifyPassword(user.PasswordHash, dto.Password))
                .Returns(true);

            // Act
            var result = await _userService.LoginAsync(dto);

            // Assert
            result.Success.Should().BeTrue();
            result.TokenOrErrorMessage.Should().NotBeNullOrEmpty();
           
        }

    }
}
