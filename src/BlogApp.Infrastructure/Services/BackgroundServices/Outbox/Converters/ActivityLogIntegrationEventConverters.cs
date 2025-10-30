using BlogApp.Application.Abstractions;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Events.BookshelfItemEvents;
using BlogApp.Domain.Events.CategoryEvents;
using BlogApp.Domain.Events.IntegrationEvents;
using BlogApp.Domain.Events.PermissionEvents;
using BlogApp.Domain.Events.PostEvents;
using BlogApp.Domain.Events.RoleEvents;
using BlogApp.Domain.Events.UserEvents;
using System.Text.Json;

namespace BlogApp.Infrastructure.Services.BackgroundServices.Outbox.Converters;

public interface IIntegrationEventConverterStrategy
{
    string EventType { get; }
    object? Convert(string payload);
}

internal abstract class ActivityLogIntegrationEventConverter<TDomainEvent> : IIntegrationEventConverterStrategy
{
    public abstract string EventType { get; }

    public object? Convert(string payload)
    {
        var domainEvent = JsonSerializer.Deserialize<TDomainEvent>(payload);
        return domainEvent is null
            ? null
            : Convert(domainEvent);
    }

    protected abstract ActivityLogCreatedIntegrationEvent Convert(TDomainEvent domainEvent);
}

internal sealed class CategoryCreatedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<CategoryCreatedEvent>
{
    public override string EventType => nameof(CategoryCreatedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(CategoryCreatedEvent domainEvent) => new(
        ActivityType: "category_created",
        EntityType: "Category",
        EntityId: domainEvent.CategoryId,
        Title: $"\"{domainEvent.Name}\" kategorisi oluşturuldu",
        Details: null,
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class CategoryUpdatedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<CategoryUpdatedEvent>
{
    public override string EventType => nameof(CategoryUpdatedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(CategoryUpdatedEvent domainEvent) => new(
        ActivityType: "category_updated",
        EntityType: "Category",
        EntityId: domainEvent.CategoryId,
        Title: $"\"{domainEvent.Name}\" kategorisi güncellendi",
        Details: null,
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class CategoryDeletedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<CategoryDeletedEvent>
{
    public override string EventType => nameof(CategoryDeletedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(CategoryDeletedEvent domainEvent) => new(
        ActivityType: "category_deleted",
        EntityType: "Category",
        EntityId: domainEvent.CategoryId,
        Title: $"\"{domainEvent.Name}\" kategorisi silindi",
        Details: null,
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class BookshelfItemCreatedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<BookshelfItemCreatedEvent>
{
    public override string EventType => nameof(BookshelfItemCreatedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(BookshelfItemCreatedEvent domainEvent) => new(
        ActivityType: "bookshelf_item_created",
        EntityType: "BookshelfItem",
        EntityId: domainEvent.ItemId,
        Title: $"\"{domainEvent.Title}\" kitaplığa eklendi",
        Details: null,
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class BookshelfItemUpdatedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<BookshelfItemUpdatedEvent>
{
    public override string EventType => nameof(BookshelfItemUpdatedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(BookshelfItemUpdatedEvent domainEvent) => new(
        ActivityType: "bookshelf_item_updated",
        EntityType: "BookshelfItem",
        EntityId: domainEvent.ItemId,
        Title: $"\"{domainEvent.Title}\" kitabı güncellendi",
        Details: null,
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class BookshelfItemDeletedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<BookshelfItemDeletedEvent>
{
    public override string EventType => nameof(BookshelfItemDeletedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(BookshelfItemDeletedEvent domainEvent) => new(
        ActivityType: "bookshelf_item_deleted",
        EntityType: "BookshelfItem",
        EntityId: domainEvent.ItemId,
        Title: $"\"{domainEvent.Title}\" kitabı silindi",
        Details: null,
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class PostCreatedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<PostCreatedEvent>
{
    public override string EventType => nameof(PostCreatedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(PostCreatedEvent domainEvent) => new(
        ActivityType: "post_created",
        EntityType: "Post",
        EntityId: domainEvent.PostId,
        Title: $"\"{domainEvent.Title}\" oluşturuldu",
        Details: $"Kategori ID: {domainEvent.CategoryId}",
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class PostUpdatedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<PostUpdatedEvent>
{
    public override string EventType => nameof(PostUpdatedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(PostUpdatedEvent domainEvent) => new(
        ActivityType: "post_updated",
        EntityType: "Post",
        EntityId: domainEvent.PostId,
        Title: $"\"{domainEvent.Title}\" güncellendi",
        Details: null,
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class PostDeletedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<PostDeletedEvent>
{
    public override string EventType => nameof(PostDeletedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(PostDeletedEvent domainEvent) => new(
        ActivityType: "post_deleted",
        EntityType: "Post",
        EntityId: domainEvent.PostId,
        Title: $"\"{domainEvent.Title}\" silindi",
        Details: null,
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class UserCreatedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<UserCreatedEvent>
{
    public override string EventType => nameof(UserCreatedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(UserCreatedEvent domainEvent) => new(
        ActivityType: "user_created",
        EntityType: "User",
        EntityId: domainEvent.UserId,
        Title: $"Kullanıcı \"{domainEvent.UserName}\" oluşturuldu",
        Details: $"Email: {domainEvent.Email}",
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class UserUpdatedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<UserUpdatedEvent>
{
    public override string EventType => nameof(UserUpdatedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(UserUpdatedEvent domainEvent) => new(
        ActivityType: "user_updated",
        EntityType: "User",
        EntityId: domainEvent.UserId,
        Title: $"Kullanıcı \"{domainEvent.UserName}\" güncellendi",
        Details: null,
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class UserDeletedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<UserDeletedEvent>
{
    public override string EventType => nameof(UserDeletedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(UserDeletedEvent domainEvent) => new(
        ActivityType: "user_deleted",
        EntityType: "User",
        EntityId: domainEvent.UserId,
        Title: $"Kullanıcı \"{domainEvent.UserName}\" silindi",
        Details: null,
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class UserRolesAssignedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<UserRolesAssignedEvent>
{
    public override string EventType => nameof(UserRolesAssignedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(UserRolesAssignedEvent domainEvent) => new(
        ActivityType: "user_roles_assigned",
        EntityType: "User",
        EntityId: domainEvent.UserId,
        Title: $"Kullanıcı \"{domainEvent.UserName}\" için roller atandı",
        Details: $"Roller: {string.Join(", ", domainEvent.RoleNames)}",
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class RoleCreatedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<RoleCreatedEvent>
{
    public override string EventType => nameof(RoleCreatedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(RoleCreatedEvent domainEvent) => new(
        ActivityType: "role_created",
        EntityType: "Role",
        EntityId: domainEvent.RoleId,
        Title: $"Rol \"{domainEvent.RoleName}\" oluşturuldu",
        Details: null,
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class RoleUpdatedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<RoleUpdatedEvent>
{
    public override string EventType => nameof(RoleUpdatedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(RoleUpdatedEvent domainEvent) => new(
        ActivityType: "role_updated",
        EntityType: "Role",
        EntityId: domainEvent.RoleId,
        Title: $"Rol \"{domainEvent.RoleName}\" güncellendi",
        Details: null,
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class RoleDeletedIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<RoleDeletedEvent>
{
    public override string EventType => nameof(RoleDeletedEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(RoleDeletedEvent domainEvent) => new(
        ActivityType: "role_deleted",
        EntityType: "Role",
        EntityId: domainEvent.RoleId,
        Title: $"Rol \"{domainEvent.RoleName}\" silindi",
        Details: null,
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}

internal sealed class PermissionsAssignedToRoleIntegrationEventConverter(IExecutionContextAccessor executionContextAccessor) : ActivityLogIntegrationEventConverter<PermissionsAssignedToRoleEvent>
{
    public override string EventType => nameof(PermissionsAssignedToRoleEvent);

    protected override ActivityLogCreatedIntegrationEvent Convert(PermissionsAssignedToRoleEvent domainEvent) => new(
        ActivityType: "permissions_assigned_to_role",
        EntityType: "Role",
        EntityId: domainEvent.RoleId,
        Title: $"Rol \"{domainEvent.RoleName}\" için yetkiler atandı",
        Details: $"{domainEvent.PermissionNames.Count} yetki atandı",
        UserId: executionContextAccessor.GetCurrentUserId() ?? SystemUsers.SystemUserId,
        Timestamp: DateTime.UtcNow
    );
}