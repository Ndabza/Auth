namespace Auth.Application.Common.Responses;

public sealed record TokenResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTime Expires { get; init; }
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime RefreshTokenExpires { get; init; }
}