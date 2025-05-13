using WeatherAPI.Models;

namespace WeatherAPI.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("users").WithTags(nameof(User));
        }
    }
}
