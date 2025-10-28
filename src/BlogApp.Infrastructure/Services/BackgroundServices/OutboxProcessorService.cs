using BlogApp.Application.Abstractions;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Repositories;
using BlogApp.Infrastructure.Services.BackgroundServices.Outbox.Converters;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BlogApp.Infrastructure.Services.BackgroundServices;

/// <summary>
/// Outbox mesajlarını işleyen ve RabbitMQ'ya yayınlayan arka plan servisi
/// Güvenilir mesaj iletimi için Outbox Pattern uygular
/// </summary>
public class OutboxProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorService> _logger;
    private readonly IReadOnlyDictionary<string, IIntegrationEventConverterStrategy> _converterStrategies;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 50;
    private const int MaxRetryCount = 5;

    public OutboxProcessorService(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorService> logger,
        IEnumerable<IIntegrationEventConverterStrategy> converterStrategies)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _converterStrategies = (converterStrategies ?? throw new ArgumentNullException(nameof(converterStrategies)))
            .GroupBy(converter => converter.EventType, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox İşleyici Servisi başlatıldı");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox mesajları işlenirken hata oluştu");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        _logger.LogInformation("Outbox İşleyici Servisi durduruldu");
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<Domain.Common.IUnitOfWork>();
        var executionContextAccessor = scope.ServiceProvider.GetRequiredService<IExecutionContextAccessor>();

        // İşlenmemiş mesajları getir
        var messages = await outboxRepository.GetUnprocessedMessagesAsync(BatchSize, cancellationToken);

        if (!messages.Any())
        {
            return; // İşlenecek mesaj yok
        }

        _logger.LogInformation("{Count} adet outbox mesajı işleniyor", messages.Count);

        using var auditScope = executionContextAccessor.BeginScope(SystemUsers.SystemUserId);

        foreach (var message in messages)
        {
            try
            {
                if (!_converterStrategies.TryGetValue(message.EventType, out var converter))
                {
                    _logger.LogWarning("Event tipi için converter bulunamadı: {EventType}", message.EventType);
                    await outboxRepository.MarkAsFailedAsync(
                        message.Id,
                        $"Bilinmeyen event tipi: {message.EventType}",
                        null,
                        cancellationToken);
                    continue;
                }

                object? integrationEvent;

                try
                {
                    integrationEvent = converter.Convert(message.Payload);
                }
                catch (Exception conversionException)
                {
                    _logger.LogError(conversionException, "{EventType} event'i dönüştürülürken hata oluştu", message.EventType);

                    await outboxRepository.MarkAsFailedAsync(
                        message.Id,
                        conversionException.Message,
                        null,
                        cancellationToken);
                    continue;
                }

                if (integrationEvent != null)
                {
                    await publishEndpoint.Publish(integrationEvent, cancellationToken);

                    await outboxRepository.MarkAsProcessedAsync(message.Id, cancellationToken);

                    _logger.LogDebug("{MessageId} ID'li {EventType} türündeki outbox mesajı başarıyla yayınlandı",
                        message.Id, message.EventType);
                }
                else
                {
                    _logger.LogWarning("{EventType} event'i dönüştürülemedi", message.EventType);

                    await outboxRepository.MarkAsFailedAsync(
                        message.Id,
                        $"Event dönüştürülemedi: {message.EventType}",
                        null,
                        cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox mesajı {MessageId} yayınlanırken hata oluştu", message.Id);

                if (message.RetryCount < MaxRetryCount)
                {
                    await outboxRepository.MarkAsFailedAsync(
                        message.Id,
                        ex.Message,
                        null,
                        cancellationToken);
                }
                else
                {
                    _logger.LogError("Mesaj {MessageId} maksimum deneme sayısını aştı. Dead letter'a taşınıyor.",
                        message.Id);
                }
            }
        }

        // Tüm batch'i tek seferde kaydet - PERFORMANS İYİLEŞTİRMESİ
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogDebug("Batch içindeki tüm mesaj durum güncellemeleri toplu olarak kaydedildi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Outbox mesaj durumları kaydedilirken hata oluştu");
            throw;
        }

        // Eski işlenmiş mesajları temizle (7 günden eski)
        try
        {
            await outboxRepository.CleanupProcessedMessagesAsync(7, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Outbox temizleme işlemi sırasında hata oluştu");
        }
    }
}
