using BlogApp.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlogApp.Infrastructure.Services.BackgroundServices;

/// <summary>
/// Süresi dolmuş veya iptal edilmiş refresh session kayıtlarını periyodik olarak temizler.
/// Performance ve veritabanı boyutunu optimize etmek için gereklidir.
/// </summary>
public class SessionCleanupService : BackgroundService
{
    private readonly ILogger<SessionCleanupService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(6); // Her 6 saatte bir

    public SessionCleanupService(
        ILogger<SessionCleanupService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Session Cleanup Service başlatıldı. Temizlik aralığı: {Interval} saat", _cleanupInterval.TotalHours);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_cleanupInterval, stoppingToken);
                await CleanupExpiredSessionsAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Session Cleanup Service durduruluyor...");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Session temizliği sırasında hata oluştu");
            }
        }
    }

    private async Task CleanupExpiredSessionsAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRefreshSessionRepository>();

            var deletedCount = await repository.DeleteExpiredSessionsAsync(cancellationToken);

            if (deletedCount > 0)
            {
                _logger.LogInformation("Süresi dolmuş {Count} refresh session kaydı temizlendi", deletedCount);
            }
            else
            {
                _logger.LogDebug("Temizlenecek süresi dolmuş session bulunamadı");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Session cleanup işlemi başarısız oldu");
        }
    }
}
