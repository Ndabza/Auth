namespace Auth.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task InsertIntoRefreshTokenAsync(Guid userId, string refreshToken, DateTime expires,
        IDbTransaction? transaction = null);

    Task<RefreshToken?> GetByRefreshTokenAsync(string refreshToken, IDbTransaction? transaction = null);
    Task RevokeAllActiveTokensAsync(Guid userId, IDbTransaction? transaction = null);
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken, IDbTransaction? transaction = null);
}