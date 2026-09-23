namespace Auth.Infrastructure.Services;

public class TokenService(IOptions<JwtOptions> options) : ITokenService
{
    public (string accessToken, DateTime expires) GenerateToken(AuthUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key));
        var expires = DateTime.UtcNow.AddMinutes(options.Value.Expires);
        var securityToken = new JwtSecurityToken(
            options.Value.Issuer,
            options.Value.Audience,
            claims,
            expires: expires,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(securityToken);

        return (accessToken, expires);
    }

    public string GenerateRefreshToken()
    {
        var number = new byte[64];

        using var random = RandomNumberGenerator.Create();
        random.GetBytes(number);

        return Convert.ToBase64String(number);
    }
}