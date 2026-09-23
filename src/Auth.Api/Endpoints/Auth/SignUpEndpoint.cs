namespace Auth.Api.Endpoints.Auth;

public static class SignUpEndpoint
{
    public static void MapSignUpEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/signup", async ([FromForm] SignUpRequest signUpRequest, IAccountService accountService) =>
        {
            if (signUpRequest.Avatar.Length == 0)
                return Results.BadRequest("Avatar is required");

            var stream = signUpRequest.Avatar.OpenReadStream();
            var dto = new SignUpDto(
                signUpRequest.Email,
                signUpRequest.FirstName,
                signUpRequest.LastName,
                signUpRequest.Bio,
                stream,
                signUpRequest.Avatar.ContentType,
                Path.GetExtension(signUpRequest.Avatar.FileName),
                signUpRequest.Password);

            await accountService.SignUpAsync(dto);

            return Results.Created("/auth/signup", "User created successfully");
        }).DisableAntiforgery();
    }
}

public sealed record SignUpRequest(
    string Email,
    string FirstName,
    string LastName,
    string Bio,
    IFormFile Avatar,
    string Password
);