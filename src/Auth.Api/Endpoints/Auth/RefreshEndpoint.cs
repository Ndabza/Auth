namespace Auth.Api.Endpoints.Auth;

public static class RefreshEndpoint
{
    public static void MapRefreshEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/refresh", async (HttpContext httpContext, IAccountService accountService) =>
        {
            var token = httpContext.Request.Cookies[GlobalConstants.RefreshTokenCookieName];

            if (token == null)
                return Results.Unauthorized();

            var results = await accountService.RefreshToken(token);

            httpContext.Response.Cookies.SetCookie(GlobalConstants.AccessTokenCookieName, results.Token,
                results.Expires);
            httpContext.Response.Cookies.SetCookie(GlobalConstants.RefreshTokenCookieName, results.RefreshToken,
                results.RefreshTokenExpires);

            return Results.Ok();
        });
    }
}