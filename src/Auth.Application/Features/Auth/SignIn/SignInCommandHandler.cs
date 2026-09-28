using MediatR;

namespace Auth.Application.Features.Auth.SignIn;

public sealed class SignInCommandHandler(
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IOptions<JwtOptions> jwtOptions):IRequestHandler<SignInCommand, TokenResponse>
{
    private const string DummyHash = "$2b$05$cBxP1KOk8hWkcfgpgqcNq.IMhILB7eCQGE9u3.N/WQyTd2Ek7GG22";
    
    public async Task<TokenResponse> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var signInRequest = request.SignInRequest;
        
        var user = await userRepository.GetUserByEmailAsync(signInRequest.Email, cancellationToken);
        var hashToVerify = user != null ? user.PasswordHash : DummyHash;

        var isValid = passwordHasher.VerifyPassword(signInRequest.Password, hashToVerify);

        if (user == null || !isValid)
            throw new UnauthorizedException("Invalid credentials");

        var refreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenExpires = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpires);
        var token = RefreshToken.Create(user.Id, refreshToken, refreshTokenExpires);
        
        await refreshTokenRepository.InsertIntoRefreshTokenAsync(token, cancellationToken);

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