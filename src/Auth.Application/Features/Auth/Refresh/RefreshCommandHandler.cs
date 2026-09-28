using MediatR;
using Microsoft.Extensions.Logging;

namespace Auth.Application.Features.Auth.Refresh;

public sealed class RefreshCommandHandler(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    ITokenService tokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IOptions<JwtOptions> jwtOptions,
    ILogger<RefreshCommandHandler> logger) : IRequestHandler<RefreshCommand, TokenResponse>
{
    public async Task<TokenResponse> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var token = request.RefreshToken;
        var existingToken = await refreshTokenRepository.GetByRefreshTokenAsync(token, cancellationToken) ??
                            throw new UnauthorizedException("Refresh token not found");

        if (!existingToken.IsActive)
        {
            if(DateTime.UtcNow >= existingToken.TokenExpires)
                throw new UnauthorizedException("Expired or invalid token.");
            
            await refreshTokenRepository.RevokeAllActiveTokensAsync(existingToken.UserId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation($"Refresh tokens have been revoked. Compromised session.");
            throw new UnauthorizedException("Compromised session detected. All sessions revoked.");
        }

        var refreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenExpires = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpires);

        
        var newToken = RefreshToken.Create(existingToken.UserId, refreshToken, refreshTokenExpires);
         existingToken.Rotate(refreshToken);
        
        await refreshTokenRepository.UpdateRefreshTokenAsync(existingToken, cancellationToken);
        await refreshTokenRepository.InsertIntoRefreshTokenAsync(newToken, cancellationToken);

        var user = await userRepository.GetUserByIdAsync(existingToken.UserId, cancellationToken);

        if (user == null) throw new UnauthorizedException("User not found");

        var (accessToken, expires) = tokenService.GenerateToken(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TokenResponse
        {
            Token = accessToken,
            Expires = expires,
            RefreshToken = refreshToken,
            RefreshTokenExpires = refreshTokenExpires
        };
    }
}