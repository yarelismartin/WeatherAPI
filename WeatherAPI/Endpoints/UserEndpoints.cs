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
                    return Results.Created("", message);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred during user registration");
                    return Results.Problem("An unexpected error occurred. Please try again later.");
                }
            });


            group.MapPost("/login", async (IUserService userService, LoginUserDTO dto) =>
            {
                try
                {
                    var (success, tokenOrMessage) = await userService.LoginAsync(dto);

                    if(!success)
                    {
                        Log.Warning("Login failed: {Message}", tokenOrMessage);
                        return Results.Conflict(tokenOrMessage);
                    }

                    return Results.Ok(new { token = tokenOrMessage });

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred during login");
                    return Results.Problem("An unexpected error occurred. Please try again later.");
                }
            });

        }
    }
}
