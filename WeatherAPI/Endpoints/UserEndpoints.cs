using WeatherAPI.DTO;
using WeatherAPI.Interfaces;
using WeatherAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Serilog;


namespace WeatherAPI.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("api/auth").WithTags("Auth");

            group.MapPost("/register", async (IUserService userService, RegisterUserDTO registerDTO) =>
            {
                try
                {
                    var (success, message) = await userService.RegisterAsync(registerDTO);

                    if (!success)
                    {
                        Log.Warning("Registration failed: {Message}", message);
                        return Results.Conflict(message);
                    }

                    Log.Information("User registered: {Email}", registerDTO.Email);
                    return Results.Ok(message);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred during user registration");
                    return Results.Problem("An unexpected error occurred. Please try again later.");
                }
            });


            group.MapPost("/login", async (IUserService userService, LoginUserDTO dto) =>
            {
                var token = await userService.LoginAsync(dto);

                if (token == null)
                {
                    return Results.Unauthorized();
                }

                return Results.Ok(new { token });
            });

        }
    }
}
