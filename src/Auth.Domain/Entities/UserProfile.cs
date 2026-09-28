namespace Auth.Domain.Entities;

public sealed class UserProfile
{
    public Guid UserId { get; private init; }
    public string FirstName { get; private init; } = string.Empty;
    public string LastName { get; private init; } = string.Empty;
    public string Bio { get; private init; } = string.Empty;
    public string AvatarUrl { get; private init; } = string.Empty;
    public DateTime UpdatedAt { get; private init; }

    private UserProfile()
    {
    }

    public static UserProfile Create(Guid userId, string firstName, string lastName, string bio, string avatarUrl,
        TimeProvider timeProvider)
    {
        if (userId == Guid.Empty) throw new ArgumentException("User Id can't be null");
        return new UserProfile
        {
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            Bio = bio,
            AvatarUrl = avatarUrl,
            UpdatedAt = timeProvider.GetUtcNow().DateTime
        };
    }
}