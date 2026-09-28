namespace Auth.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<Guid> CreateUserAsync(AuthUser authUser, CancellationToken cancellationToken);
    Task<AuthUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<AuthUser?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
}