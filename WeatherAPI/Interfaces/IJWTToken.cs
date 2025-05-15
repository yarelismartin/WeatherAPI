using System.Security.Claims;

namespace WeatherAPI.Interfaces
{
    public interface IJWTToken
    {
        string GenerateJWTToken(string userId, string email);

    }
}
