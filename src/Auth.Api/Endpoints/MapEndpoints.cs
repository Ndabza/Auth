using Auth.Api.Endpoints.Movie;

namespace Auth.Api.Endpoints;

public static class MapEndpoints
{
    public static void MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapSignUpEndpoint();
        app.MapSignInEndpoint();
        app.MapRefreshEndpoint();
        app.MapMovieEndpoint();
    }
}