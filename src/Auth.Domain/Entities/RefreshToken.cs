namespace Auth.Domain.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Token { get; init; } = string.Empty;
    public DateTime TokenExpires { get; init; }
    public DateTime? Revoked { get; set; }
    public string? ReplacedBy { get; set; } = string.Empty;
    private bool IsExpired => DateTime.UtcNow >= TokenExpires;
    private bool IsRevoked => Revoked != null;
    public bool IsActive => !IsExpired && (!IsRevoked || IsWithGracePeriod());

    private bool IsWithGracePeriod()
    {
        return IsRevoked && Revoked != null && Revoked.Value.AddSeconds(5) > DateTime.UtcNow;
    }
}