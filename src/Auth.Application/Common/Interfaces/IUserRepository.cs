namespace Auth.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<Guid> CreateUserAsync(string email, string passwordHash, IDbTransaction? transaction = null);
    Task<AuthUser?> GetUserByEmailAsync(string email, IDbTransaction? transaction = null);
    Task<AuthUser?> GetUserByIdAsync(Guid userId, IDbTransaction? transaction = null);
}