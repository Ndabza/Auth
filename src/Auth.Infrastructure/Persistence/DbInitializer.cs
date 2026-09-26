using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure.Persistence
{
    public class DbInitializer : IDbInitializer
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

                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                // Check if any user tables already exist in the public schema
                string checkTablesSql = @"
                    SELECT EXISTS (
                        SELECT FROM information_schema.tables 
                        WHERE table_schema = 'public' 
                        AND table_type = 'BASE TABLE'
                    );";

                bool tablesExist = await connection.ExecuteScalarAsync<bool>(checkTablesSql);

                if (tablesExist)
                {
                    _logger.LogInformation("Database tables already exist. Skipping initialization script.");
                    return;
                }

                string sqlFilePath = Path.Combine(AppContext.BaseDirectory, "init.sql");
                if (!File.Exists(sqlFilePath))
                {
                    _logger.LogWarning($"Database initialization script not found at target path: {sqlFilePath}");
                    return;
                }

                string sqlScript = await File.ReadAllTextAsync(sqlFilePath);

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
}
