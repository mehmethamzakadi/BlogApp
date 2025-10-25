using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlogApp.Infrastructure.Services;

/// <summary>
/// Background service that periodically cleans up old logs from the database
/// </summary>
public class LogCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LogCleanupService> _logger;
    private readonly TimeSpan _cleanupInterval;
    private readonly int _logRetentionDays;

    public LogCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<LogCleanupService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;

        // Run cleanup daily at 3 AM
        _cleanupInterval = TimeSpan.FromHours(24);

        // Default retention: 90 days for Logs table
        _logRetentionDays = configuration.GetValue<int>("Logging:Database:RetentionDays", 90);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("LogCleanupService started. Retention: {RetentionDays} days", _logRetentionDays);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOldLogsAsync(stoppingToken);

                // Calculate next run time (3 AM next day)
                var now = DateTime.UtcNow;
                var next3AM = now.Date.AddDays(1).AddHours(3);
                var delay = next3AM - now;

                _logger.LogInformation("Next log cleanup scheduled for: {NextRun}", next3AM);
                await Task.Delay(delay, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during log cleanup");
                // Wait 1 hour before retry on error
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }

    private async Task CleanupOldLogsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BlogAppDbContext>();

        var cutoffDate = DateTime.UtcNow.AddDays(-_logRetentionDays);

        try
        {
            // Clean up old Serilog logs
            var deletedCount = await dbContext.Database
                .ExecuteSqlRawAsync(
                    "DELETE FROM \"Logs\" WHERE raise_date < {0}",
                    cutoffDate,
                    cancellationToken);

            if (deletedCount > 0)
            {
                _logger.LogInformation(
                    "Cleaned up {Count} old log entries older than {CutoffDate}",
                    deletedCount,
                    cutoffDate);
            }

            // Optional: Vacuum the table to reclaim disk space (PostgreSQL specific)
            await dbContext.Database.ExecuteSqlRawAsync("VACUUM ANALYZE \"Logs\"", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old logs");
            throw;
        }
    }
}
