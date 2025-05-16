using WeatherAPI.DTO;
using WeatherAPI.Models;

namespace WeatherAPI.Interfaces
{
    public interface IUserService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterUserDTO registerDTO);

        Task<(bool Success, string TokenOrErrorMessage)> LoginAsync(LoginUserDTO dto);
    }
}
