using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using WeatherAPI.DTO;
using WeatherAPI.Interfaces;
using WeatherAPI.Models;

namespace WeatherAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJWTToken _jwtToken;

        public UserService(IUserRepository userRepository, IJWTToken jwtToken)
        {
            _userRepository = userRepository;
            _jwtToken = jwtToken;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterUserDTO dto)
        {
            if (await _userRepository.UserEmailInUseAsync(dto.Email))
            {
                return (false, "Username already exists.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hashedPassword
            };

            await _userRepository.CreateUserAsync(user);
            return (true, "User registered successfully.");
        }

        public async Task<string?> LoginAsync(LoginUserDTO dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if(user == null || !_userRepository.VerifyPassword(user.PasswordHash, dto.Password))
        {
                return null; // Unauthorized
            }

            return _jwtToken.GenerateJWTToken(user.Id.ToString(), user.Email);
        }

    }
}
