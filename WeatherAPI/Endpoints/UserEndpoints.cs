using WeatherAPI.DTO;
using WeatherAPI.Interfaces;
using WeatherAPI.Models;
using Microsoft.AspNetCore.Authorization;


namespace WeatherAPI.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("api/auth").WithTags("Auth");

            group.MapPost("/register", async (IUserService userService, RegisterUserDTO registerDTO) =>
            {
                var (success, message) = await userService.RegisterAsync(registerDTO);

                if (!success)
                    return Results.Conflict(message);

                return Results.Ok(message);
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
