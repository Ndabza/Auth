using Auth.Application.Features.Auth.SignUp;
using MediatR;

namespace Auth.Api.Endpoints.Auth;

public static class SignUpEndpoint
{
    private const string BlobStorageContainerName = "auth-container";

    public static void MapSignUpEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/signup",
            async ([FromForm] SignUpRequest signUpRequest, IStorageService storageService, ISender mediator) =>
            {
                var avatarUrl = string.Empty;

                if (signUpRequest.Avatar.Length > 0)
                {
                    await using var stream = signUpRequest.Avatar.OpenReadStream();
                    avatarUrl = await storageService.UpLoadFileAsync(
                        BlobStorageContainerName,
                        stream,
                        signUpRequest.Avatar.ContentType,
                        Path.GetExtension(signUpRequest.Avatar.FileName));
                }

                try
                {
                    var dto = new SignUpDto(
                        signUpRequest.Email,
                        signUpRequest.FirstName,
                        signUpRequest.LastName,
                        signUpRequest.Bio,
                        avatarUrl,
                        signUpRequest.Password);

                    await mediator.Send(new SignUpCommand(dto));

                    return Results.Created("/auth/signup", "User created successfully");
                }
                catch (Exception)
                {
                    if (!string.IsNullOrEmpty(avatarUrl))
                        await storageService.DeleteAsync(BlobStorageContainerName, avatarUrl);
                    throw;
                }
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