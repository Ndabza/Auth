namespace Auth.Domain.Entities;

public sealed class AuthUser
{
    public Guid Id { get; private init; }
    public string Email { get; private init; } = string.Empty;
    public string PasswordHash { get; private init; } = string.Empty;
    public string Role { get; private init; } = string.Empty;
    public DateTime CreatedAt { get; private init; }
    public DateTime UpdatedAt { get; private init; }

    private AuthUser()
    {}

    public static AuthUser Create(string email, string passwordHash, TimeProvider timeProvider)
    {
        var now = timeProvider.GetUtcNow().DateTime;
        return new AuthUser
        {
           Id =  Guid.NewGuid(),
           Email =  email,
           PasswordHash =   passwordHash,
           CreatedAt = now,
           UpdatedAt = now
        };
    }
}