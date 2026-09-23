namespace Auth.Application.Common.Interfaces;

public interface ITokenService
{
    (string accessToken, DateTime expires) GenerateToken(AuthUser user);
    string GenerateRefreshToken();
}