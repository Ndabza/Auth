namespace Auth.Domain.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; private init; }
    public Guid UserId { get; private init; }
    public string Token { get; private init; } = string.Empty;
    public DateTime TokenExpires { get; private init; }
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; } = string.Empty;
    private bool IsExpired => DateTime.UtcNow >= TokenExpires;
    private bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsExpired && (!IsRevoked || IsWithGracePeriod());

    private bool IsWithGracePeriod() => RevokedAt != null && RevokedAt.Value.AddSeconds(5) > DateTime.UtcNow;

    public static RefreshToken Create(Guid userId, string token, DateTime tokenExpires) => new()
    {
        Id = Guid.NewGuid(),
        UserId =  userId,
        Token =  token,
        TokenExpires =  tokenExpires
    };

    public void Rotate(string newReplacedByToken)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = newReplacedByToken;
    }
}