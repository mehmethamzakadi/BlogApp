using BlogApp.Domain.Events.CategoryEvents;
using BlogApp.Domain.Events.IntegrationEvents;
using BlogApp.Domain.Events.PermissionEvents;
using BlogApp.Domain.Events.PostEvents;
using BlogApp.Domain.Events.RoleEvents;
using BlogApp.Domain.Events.UserEvents;
using BlogApp.Domain.Repositories;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BlogApp.Infrastructure.Services.BackgroundServices;

/// <summary>
/// Background service that processes outbox messages and publishes them to RabbitMQ
/// Implements the Outbox Pattern for reliable message delivery
/// </summary>
public class OutboxProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorService> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 50;
    private const int MaxRetryCount = 5;

    public OutboxProcessorService(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing outbox messages");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        _logger.LogInformation("Outbox Processor Service stopped");
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
        var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<Domain.Common.IUnitOfWork>();

        // Get unprocessed messages
        var messages = await outboxRepository.GetUnprocessedMessagesAsync(BatchSize, cancellationToken);

        if (!messages.Any())
        {
            return; // No messages to process
        }

        _logger.LogInformation("Processing {Count} outbox messages", messages.Count);

        foreach (var message in messages)
        {
            try
            {
                // Deserialize and publish the event
                var integrationEvent = DeserializeEvent(message.EventType, message.Payload);

                if (integrationEvent != null)
                {
                    await publishEndpoint.Publish(integrationEvent, cancellationToken);

                    // Mark as processed
                    await outboxRepository.MarkAsProcessedAsync(message.Id, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    _logger.LogDebug("Successfully published outbox message {MessageId} of type {EventType}",
                        message.Id, message.EventType);
                }
                else
                {
                    _logger.LogWarning("Could not deserialize event type: {EventType}", message.EventType);
                    await outboxRepository.MarkAsFailedAsync(
                        message.Id,
                        $"Unknown event type: {message.EventType}",
                        null,
                        cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing outbox message {MessageId}", message.Id);

                if (message.RetryCount < MaxRetryCount)
                {
                    await outboxRepository.MarkAsFailedAsync(
                        message.Id,
                        ex.Message,
                        null,
                        cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    _logger.LogError("Message {MessageId} exceeded max retry count. Moving to dead letter.",
                        message.Id);
                }
            }
        }

        // Cleanup old processed messages (older than 7 days)
        try
        {
            await outboxRepository.CleanupProcessedMessagesAsync(7, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during outbox cleanup");
        }
    }

    private object? DeserializeEvent(string eventType, string payload)
    {
        try
        {
            return eventType switch
            {
                // Category Events
                "CategoryCreatedEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<CategoryCreatedEvent>(payload)),
                "CategoryUpdatedEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<CategoryUpdatedEvent>(payload)),
                "CategoryDeletedEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<CategoryDeletedEvent>(payload)),

                // Post Events
                "PostCreatedEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<PostCreatedEvent>(payload)),
                "PostUpdatedEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<PostUpdatedEvent>(payload)),
                "PostDeletedEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<PostDeletedEvent>(payload)),

                // User Events
                "UserCreatedEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<UserCreatedEvent>(payload)),
                "UserUpdatedEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<UserUpdatedEvent>(payload)),
                "UserDeletedEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<UserDeletedEvent>(payload)),
                "UserRolesAssignedEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<UserRolesAssignedEvent>(payload)),

                // Role Events
                "RoleCreatedEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<RoleCreatedEvent>(payload)),
                "RoleUpdatedEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<RoleUpdatedEvent>(payload)),
                "RoleDeletedEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<RoleDeletedEvent>(payload)),

                // Permission Events
                "PermissionsAssignedToRoleEvent" => ConvertToIntegrationEvent(
                    JsonSerializer.Deserialize<PermissionsAssignedToRoleEvent>(payload)),

                _ => null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing event type {EventType}", eventType);
            return null;
        }
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(CategoryCreatedEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "category_created",
            EntityType: "Category",
            EntityId: domainEvent.CategoryId,
            Title: $"\"{domainEvent.Name}\" kategorisi oluşturuldu",
            Details: null,
            UserId: domainEvent.CreatedById,
            Timestamp: DateTime.UtcNow
        );
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(CategoryUpdatedEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "category_updated",
            EntityType: "Category",
            EntityId: domainEvent.CategoryId,
            Title: $"\"{domainEvent.Name}\" kategorisi güncellendi",
            Details: null,
            UserId: domainEvent.UpdatedById,
            Timestamp: DateTime.UtcNow
        );
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(CategoryDeletedEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "category_deleted",
            EntityType: "Category",
            EntityId: domainEvent.CategoryId,
            Title: $"\"{domainEvent.Name}\" kategorisi silindi",
            Details: null,
            UserId: domainEvent.DeletedById,
            Timestamp: DateTime.UtcNow
        );
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(PostCreatedEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "post_created",
            EntityType: "Post",
            EntityId: domainEvent.PostId,
            Title: $"\"{domainEvent.Title}\" oluşturuldu",
            Details: $"Kategori ID: {domainEvent.CategoryId}",
            UserId: domainEvent.CreatedById,
            Timestamp: DateTime.UtcNow
        );
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(PostUpdatedEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "post_updated",
            EntityType: "Post",
            EntityId: domainEvent.PostId,
            Title: $"\"{domainEvent.Title}\" güncellendi",
            Details: null,
            UserId: domainEvent.UpdatedById,
            Timestamp: DateTime.UtcNow
        );
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(PostDeletedEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "post_deleted",
            EntityType: "Post",
            EntityId: domainEvent.PostId,
            Title: $"\"{domainEvent.Title}\" silindi",
            Details: null,
            UserId: domainEvent.DeletedById,
            Timestamp: DateTime.UtcNow
        );
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(UserCreatedEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "user_created",
            EntityType: "User",
            EntityId: domainEvent.UserId,
            Title: $"Kullanıcı \"{domainEvent.UserName}\" oluşturuldu",
            Details: $"Email: {domainEvent.Email}",
            UserId: domainEvent.CreatedById,
            Timestamp: DateTime.UtcNow
        );
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(UserUpdatedEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "user_updated",
            EntityType: "User",
            EntityId: domainEvent.UserId,
            Title: $"Kullanıcı \"{domainEvent.UserName}\" güncellendi",
            Details: null,
            UserId: domainEvent.UpdatedById,
            Timestamp: DateTime.UtcNow
        );
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(UserDeletedEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "user_deleted",
            EntityType: "User",
            EntityId: domainEvent.UserId,
            Title: $"Kullanıcı \"{domainEvent.UserName}\" silindi",
            Details: null,
            UserId: domainEvent.DeletedById,
            Timestamp: DateTime.UtcNow
        );
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(UserRolesAssignedEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "user_roles_assigned",
            EntityType: "User",
            EntityId: domainEvent.UserId,
            Title: $"Kullanıcı \"{domainEvent.UserName}\" için roller atandı",
            Details: $"Roller: {string.Join(", ", domainEvent.RoleNames)}",
            UserId: domainEvent.AssignedById,
            Timestamp: DateTime.UtcNow
        );
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(RoleCreatedEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "role_created",
            EntityType: "Role",
            EntityId: domainEvent.RoleId,
            Title: $"Rol \"{domainEvent.RoleName}\" oluşturuldu",
            Details: null,
            UserId: domainEvent.CreatedById,
            Timestamp: DateTime.UtcNow
        );
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(RoleUpdatedEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "role_updated",
            EntityType: "Role",
            EntityId: domainEvent.RoleId,
            Title: $"Rol \"{domainEvent.RoleName}\" güncellendi",
            Details: null,
            UserId: domainEvent.UpdatedById,
            Timestamp: DateTime.UtcNow
        );
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(RoleDeletedEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "role_deleted",
            EntityType: "Role",
            EntityId: domainEvent.RoleId,
            Title: $"Rol \"{domainEvent.RoleName}\" silindi",
            Details: null,
            UserId: domainEvent.DeletedById,
            Timestamp: DateTime.UtcNow
        );
    }

    private static ActivityLogCreatedIntegrationEvent? ConvertToIntegrationEvent(PermissionsAssignedToRoleEvent? domainEvent)
    {
        if (domainEvent == null) return null;

        return new ActivityLogCreatedIntegrationEvent(
            ActivityType: "permissions_assigned_to_role",
            EntityType: "Role",
            EntityId: domainEvent.RoleId,
            Title: $"Rol \"{domainEvent.RoleName}\" için yetkiler atandı",
            Details: $"{domainEvent.PermissionNames.Count} yetki atandı",
            UserId: domainEvent.AssignedById,
            Timestamp: DateTime.UtcNow
        );
    }
}
