using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure.Persistence;

public class DbInitializer:IDbInitializer
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DbInitializer> _logger;

    public DbInitializer(IConfiguration configuration, ILogger<DbInitializer> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            string sqlFilePath = Path.Combine(AppContext.BaseDirectory, "init.sql");

            if (!File.Exists(sqlFilePath))
            {
                _logger.LogWarning($"Database initialization script not found at target path: {sqlFilePath}");
                return;
            }

            string sqlScript = await File.ReadAllTextAsync(sqlFilePath);

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            _logger.LogInformation("Initializing database tables via Dapper infrastructure service...");
            await connection.ExecuteAsync(sqlScript);
            _logger.LogInformation("Database tables initialized successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing the Dapper database initialization script.");
            throw;
        }
    }

}