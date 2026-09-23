namespace Auth.Application.Common.Requests;

public sealed record SignInRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}