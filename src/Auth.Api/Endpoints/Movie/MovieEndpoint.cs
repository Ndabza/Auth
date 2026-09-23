namespace Auth.Api.Endpoints.Movie;

public static class MovieEndpoint
{
    public static void MapMovieEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/movies", () =>
        {
            var movies = new List<string>
            {
                "John Wick 2",
                "Spiderman home coming",
                "Iron Man 2",
                "The incredible hulk"
            };
            return Results.Ok(movies);
        }).RequireAuthorization();
    }
}