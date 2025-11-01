using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.IntegrationEvents;
using BlogApp.Domain.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BlogApp.Infrastructure.Consumers;

/// <summary>
/// Consumes ActivityLogCreatedIntegrationEvent from RabbitMQ
/// and persists the activity log to the database
/// </summary>
public class ActivityLogConsumer : IConsumer<ActivityLogCreatedIntegrationEvent>
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivityLogConsumer> _logger;

    public ActivityLogConsumer(
        IActivityLogRepository activityLogRepository,
        IUnitOfWork unitOfWork,
        ILogger<ActivityLogConsumer> logger)
    {
        _activityLogRepository = activityLogRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ActivityLogCreatedIntegrationEvent> context)
    {
        try
        {
            var message = context.Message;

            // ✅ Idempotency kontrolü - MessageId kullan (MassTransit otomatik oluşturur)
            // Eğer MessageId yoksa (çok nadir), EntityId + Timestamp kombinasyonunu hash'le
            Guid activityLogId;
            
            if (context.MessageId.HasValue)
            {
                activityLogId = context.MessageId.Value;
            }
            else
            {
                // Fallback: Deterministic ID oluştur (EntityId + Timestamp + ActivityType)
                var deterministicString = $"{message.EntityId}_{message.Timestamp:O}_{message.ActivityType}";
                activityLogId = GenerateDeterministicGuid(deterministicString);
                
                _logger.LogWarning(
                    "MessageId not available, generated deterministic ID: {ActivityLogId}",
                    activityLogId);
            }

            // Daha önce işlenmiş mi kontrol et
            var exists = await _activityLogRepository.ExistsByIdAsync(activityLogId, context.CancellationToken);
            if (exists)
            {
                _logger.LogInformation(
                    "Duplicate message detected for ActivityLog {ActivityLogId}. Skipping processing (idempotent).",
                    activityLogId);
                return; // ✅ Idempotent - zaten işlenmiş, tekrar işleme
            }

            _logger.LogInformation(
                "Processing ActivityLog: {ActivityType} for {EntityType} (ID: {EntityId})",
                message.ActivityType,
                message.EntityType,
                message.EntityId);

            var activityLog = new ActivityLog
            {
                Id = activityLogId, // ✅ Deterministic ID
                ActivityType = message.ActivityType,
                EntityType = message.EntityType,
                EntityId = message.EntityId,
                Title = message.Title,
                Details = message.Details,
                UserId = message.UserId ?? Guid.Empty,
                Timestamp = message.Timestamp
            };

            await _activityLogRepository.AddAsync(activityLog, context.CancellationToken);
            
            // ✅ UnitOfWork ile transaction yönetimi
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation(
                "Successfully processed ActivityLog: {ActivityType} for {EntityType}",
                message.ActivityType,
                message.EntityType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing ActivityLog: {ActivityType}",
                context.Message.ActivityType);
            throw; // Let MassTransit handle retry logic
        }
    }

    /// <summary>
    /// String'den deterministic Guid oluşturur (MD5 hash kullanarak)
    /// Aynı string her zaman aynı Guid'i üretir
    /// </summary>
    private static Guid GenerateDeterministicGuid(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return new Guid(hash);
    }
}
