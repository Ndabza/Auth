namespace Auth.Application.Common.Interfaces;

public interface IUserProfileRepository
{
    Task CreateProfileAsync(UserProfile userProfile, CancellationToken cancellationToken);

    Task<UserProfile> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}