using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using WeatherAPI.DTO;
using WeatherAPI.Interfaces;
using WeatherAPI.Models;
using Serilog;

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
                Log.Warning("Attempt to register with already used email: {Email}", dto.Email);
                return (false, "This email is already in use. Please try a different email.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hashedPassword,
                Email = dto.Email,
            };

            try
            {

                var result = await _userRepository.CreateUserAsync(user);
                Log.Information("User created successfully: {@User}", result);

                return (true, "User registered successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in RegisterAsync for email: {Email}", dto.Email);
                throw; 
            }
        }

        public async Task<(bool Success, string TokenOrErrorMessage)> LoginAsync(LoginUserDTO dto)
        {
            if(!await _userRepository.UserEmailInUseAsync(dto.Email))
            {
                Log.Warning("Invalid login attempt — email or password incorrect.");
                return (false, "This email has not been registered in the database. Please try a different email");
            }

            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null || !_userRepository.VerifyPassword(user.PasswordHash, dto.Password))
            {
                Log.Warning("Invalid email or password.", dto.Email);
                return (false, "Invalid email or password.");
            }

            try
            {
                var token = _jwtToken.GenerateJWTToken(user.Id.ToString(), user.Email);

                Log.Information("User {Email} successfully logged in and token was issued.", user.Email);
                return (true, token);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in LoginAsync for email: {Email}", dto.Email);
                throw;
            }
        }

    }
}
