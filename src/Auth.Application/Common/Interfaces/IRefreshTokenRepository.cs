namespace Auth.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task InsertIntoRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<RefreshToken?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task RevokeAllActiveTokensAsync(Guid userId, CancellationToken cancellationToken);
    Task UpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
}