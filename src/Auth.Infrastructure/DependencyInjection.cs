namespace Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services,
        IConfiguration configuration)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddScoped<IDataAccess, DataAccess>();
        services.AddScoped<IDbInitializer, DbInitializer>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IStorageService, AzureBlobStorageService>();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.JwtOptionsKey));

        return services;
    }
}