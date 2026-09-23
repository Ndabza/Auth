namespace Auth.Application.Common.Interfaces;

public interface IUserProfileRepository
{
    Task CreateProfileAsync(Guid userId, string firstName, string lastName, string bio, string avatarUrl,
        IDbTransaction? transaction = null);

    Task<UserProfile> GetProfileByUserIdAsync(Guid userId, IDbTransaction? transaction = null);
}