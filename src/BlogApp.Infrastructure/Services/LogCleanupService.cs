using BlogApp.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlogApp.Infrastructure.Services;

/// <summary>
/// Veritabanından eski logları periyodik olarak temizleyen background service
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

        // Her gün saat 3'te temizleme işlemini çalıştır
        _cleanupInterval = TimeSpan.FromHours(24);

        // Varsayılan saklama süresi: Logs tablosu için 90 gün
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

                // Sonraki çalışma zamanını hesapla (ertesi gün saat 3)
                var now = DateTime.UtcNow;
                var next3AM = now.Date.AddDays(1).AddHours(3);
                var delay = next3AM - now;

                _logger.LogInformation("Next log cleanup scheduled for: {NextRun}", next3AM);
                await Task.Delay(delay, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during log cleanup");
                // Hata durumunda yeniden denemeden önce 1 saat bekle
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
            // Eski Serilog loglarını temizle
            var deletedCount = await dbContext.Database
                .ExecuteSqlAsync(
                    $"DELETE FROM \"Logs\" WHERE raise_date < {cutoffDate}",
                    cancellationToken);

            if (deletedCount > 0)
            {
                _logger.LogInformation(
                    "Cleaned up {Count} old log entries older than {CutoffDate}",
                    deletedCount,
                    cutoffDate);
            }

            // Optional: Vacuum the table to reclaim disk space (PostgreSQL specific)
            await dbContext.Database.ExecuteSqlRawAsync("VACUUM ANALYZE \"Logs\"");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old logs");
            throw;
        }
    }
}
