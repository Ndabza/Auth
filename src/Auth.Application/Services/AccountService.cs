namespace Auth.Application.Services;

public sealed class AccountService(
    IDataAccess dataAccess,
    ILogger<AccountService> logger,
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IStorageService storageService,
    IUserProfileRepository userProfileRepository,
    ITokenService tokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IOptions<JwtOptions> jwtOptions) : IAccountService
{
    // ReSharper disable once StringLiteralTypo
    private const string DummyHash = "$2b$05$cBxP1KOk8hWkcfgpgqcNq.IMhILB7eCQGE9u3.N/WQyTd2Ek7GG22";

    public async Task SignUpAsync(SignUpDto signUpDto)
    {
        await dataAccess.BeginTransactionAsync();

        var user = await userRepository.GetUserByEmailAsync(signUpDto.Email);

        if (user != null)
            throw new ConflictException($"User with email: '{signUpDto.Email}' already exists.'");

        var hash = passwordHasher.HashPassword(signUpDto.Password);
        var avatarUrl = string.Empty;

        try
        {
            avatarUrl = await storageService.UpLoadFileAsync("authcontainer", signUpDto.AvatarStream,
                signUpDto.ContentType, signUpDto.Extension);

            await signUpDto.AvatarStream.DisposeAsync();

            var userId = await userRepository.CreateUserAsync(signUpDto.Email, hash);

            await userProfileRepository.CreateProfileAsync(userId, signUpDto.FirstName, signUpDto.LastName,
                signUpDto.Bio, avatarUrl);

            await dataAccess.CommitAsync();
        }
        catch (Exception ex)
        {
            logger.LogError("Error occured: {message}", ex);

            await dataAccess.RollbackAsync();
            await signUpDto.AvatarStream.DisposeAsync();
            if(avatarUrl is not null && !string.IsNullOrWhiteSpace(avatarUrl))
                await storageService.DeleteAsync("authcontainer", avatarUrl);

            throw;
        }
    }

    public async Task<TokenResponse> SignInAsync(SignInRequest signInRequest)
    {
        var user = await userRepository.GetUserByEmailAsync(signInRequest.Email);
        var hashToVerify = user != null ? user.PasswordHash : DummyHash;

        var isValid = passwordHasher.VerifyPassword(signInRequest.Password, hashToVerify);

        if (user == null || !isValid)
            throw new UnauthorizedException("Invalid credentials");

        var refreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenExpires = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpires);

        await refreshTokenRepository.InsertIntoRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpires);

        var (accessToken, expires) = tokenService.GenerateToken(user);

        return new TokenResponse
        {
            Token = accessToken,
            Expires = expires,
            RefreshToken = refreshToken,
            RefreshTokenExpires = refreshTokenExpires
        };
    }

    public async Task<TokenResponse> RefreshToken(string refreshToken)
    {
        await dataAccess.BeginTransactionAsync();

        try
        {
            var existingToken =
                await refreshTokenRepository.GetByRefreshTokenAsync(refreshToken, dataAccess.Transaction);

            if (existingToken == null) throw new UnauthorizedException("Refresh token not found");

            if (!existingToken.IsActive && !string.IsNullOrWhiteSpace(existingToken.ReplacedBy))
            {
                await refreshTokenRepository.RevokeAllActiveTokensAsync(existingToken.UserId, dataAccess.Transaction);
                await dataAccess.CommitAsync();
                throw new UnauthorizedException("Compromised session detected. All sessions revoked.");
            }

            if (!existingToken.IsActive) throw new UnauthorizedException("Expired or invalid token.");

            var newRefreshToken = tokenService.GenerateRefreshToken();
            existingToken.ReplacedBy = newRefreshToken;
            var newRefreshTokenExpires = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpires);

            existingToken.Revoked = DateTime.UtcNow;
            existingToken.ReplacedBy = newRefreshToken;

            await refreshTokenRepository.UpdateRefreshTokenAsync(existingToken, dataAccess.Transaction);
            await refreshTokenRepository.InsertIntoRefreshTokenAsync(existingToken.UserId, newRefreshToken,
                newRefreshTokenExpires, dataAccess.Transaction);

            var user = await userRepository.GetUserByIdAsync(existingToken.UserId, dataAccess.Transaction);

            if (user == null) throw new UnauthorizedException("User not found");

            var (accessToken, expires) = tokenService.GenerateToken(user);

            await dataAccess.CommitAsync();

            return new TokenResponse
            {
                Token = accessToken,
                Expires = expires,
                RefreshToken = newRefreshToken,
                RefreshTokenExpires = newRefreshTokenExpires
            };
        }
        catch (Exception ex)
        {
            logger.LogError("Error occured: {message}", ex);
            await dataAccess.RollbackAsync();
            throw;
        }
    }

    public Task SignOutAsync()
    {
        throw new NotImplementedException();
    }
}