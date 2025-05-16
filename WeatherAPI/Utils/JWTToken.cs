using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherAPI.Interfaces;

namespace WeatherAPI.Utils
{
    public class JWTToken : IJWTToken
    {
        private readonly IConfiguration _configuration;

        // Constructor to inject IConfiguration for accessing appsettings.json
        public JWTToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJWTToken(string userId, string email)
        {
            // Creating claims for the JWT token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),  // Using userId as a claim
                new Claim(ClaimTypes.Name, email)  // Using email as a claim
            };

            // Creating the JWT token
            var jwtToken = new JwtSecurityToken(
                claims: claims,  // Adding claims
                notBefore: DateTime.UtcNow,  
                expires: DateTime.UtcNow.AddDays(30), 
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["ApplicationSettings:JWT_Secret"])), 
                    SecurityAlgorithms.HmacSha256Signature 
                )
            );

            // Returning the token as a string
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
