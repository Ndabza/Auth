using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Auth.Infrastructure.BackgroundServices;

public sealed class TokenCleanupService(ILogger<TokenCleanupService> logger, IServiceProvider serviceProvider) : BackgroundService
{
    private readonly TimeSpan _scheduledTime = new TimeSpan(2, 0, 0);
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var nextRun = now.Date.Add(_scheduledTime);

            if(now > nextRun) nextRun = nextRun.AddDays(1);

            TimeSpan delay = nextRun - now;

            await Task.Delay(delay, stoppingToken);

            try
            {
                using var scope = serviceProvider.CreateScope();
                var refreshTokenRepository = scope.ServiceProvider.GetRequiredService<IRefreshTokenRepository>();
                int deletedCount = await refreshTokenRepository.DeleteExpiredTokens(stoppingToken);
                logger.LogInformation("Cleaned up {Count} expired tokens.", deletedCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while cleaning up expired tokens.");
            }
        }
    }
}
