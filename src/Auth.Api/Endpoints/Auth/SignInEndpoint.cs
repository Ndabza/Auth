using Auth.Application.Features.Auth.SignIn;
using MediatR;

namespace Auth.Api.Endpoints.Auth;

public static class SignInEndpoint
{
    public static void MapSignInEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/signin",
            async (SignInRequest signInRequest, ISender mediator, HttpContext httpContext) =>
            {
                var results = await mediator.Send(new SignInCommand(signInRequest));

                httpContext.Response.Cookies.SetCookie(GlobalConstants.AccessTokenCookieName, results.Token,
                    results.Expires);
                httpContext.Response.Cookies.SetCookie(GlobalConstants.RefreshTokenCookieName, results.RefreshToken,
                    results.RefreshTokenExpires);

                return Results.Ok(new { message = "SignIn successful." });
            });
    }
}