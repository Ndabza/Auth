namespace Auth.Domain.Options;

public sealed class JwtOptions
{
    public const string JwtOptionsKey = "JwtOptions";
    public string Key { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public int Expires { get; init; }
    public int RefreshTokenExpires { get; init; }
}