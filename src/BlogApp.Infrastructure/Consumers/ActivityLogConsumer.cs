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

            _logger.LogInformation(
                "Processing ActivityLog: {ActivityType} for {EntityType} (ID: {EntityId})",
                message.ActivityType,
                message.EntityType,
                message.EntityId);

            var activityLog = new ActivityLog
            {
                ActivityType = message.ActivityType,
                EntityType = message.EntityType,
                EntityId = message.EntityId,
                Title = message.Title,
                Details = message.Details,
                UserId = message.UserId ?? Guid.Empty,
                Timestamp = message.Timestamp
            };

            await _activityLogRepository.AddAsync(activityLog, context.CancellationToken);

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
}
