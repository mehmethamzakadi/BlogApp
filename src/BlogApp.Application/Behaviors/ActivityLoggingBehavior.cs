using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

namespace BlogApp.Application.Behaviors;

/// <summary>
/// Automatically logs activities for specific command types
/// </summary>
public class ActivityLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ActivityLoggingBehavior(
        IActivityLogRepository activityLogRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _activityLogRepository = activityLogRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var response = await next();

        // Log activity after successful execution
        await LogActivityAsync(request, response, cancellationToken);

        return response;
    }

    private async Task LogActivityAsync(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest);
        var requestName = requestType.Name;

        // Determine activity type based on command name
        var (activityType, entityType, shouldLog) = DetermineActivityType(requestName);

        if (!shouldLog) return;

        var userId = GetCurrentUserId();
        var (entityId, title) = ExtractEntityInfo(request, requestName);

        var activityLog = new ActivityLog
        {
            ActivityType = activityType,
            EntityType = entityType,
            EntityId = entityId,
            Title = title,
            Details = JsonSerializer.Serialize(new { Request = requestName }),
            UserId = userId,
            Timestamp = System.DateTime.UtcNow
        };

        await _activityLogRepository.AddAsync(activityLog);
    }

    private (string ActivityType, string EntityType, bool ShouldLog) DetermineActivityType(string requestName)
    {
        return requestName switch
        {
            var name when name.Contains("CreatePost") => ("post_created", "Post", true),
            var name when name.Contains("UpdatePost") => ("post_updated", "Post", true),
            var name when name.Contains("DeletePost") => ("post_deleted", "Post", true),
            var name when name.Contains("CreateCategory") => ("category_created", "Category", true),
            var name when name.Contains("UpdateCategory") => ("category_updated", "Category", true),
            var name when name.Contains("DeleteCategory") => ("category_deleted", "Category", true),
            var name when name.Contains("CreateAppUser") => ("user_created", "User", true),
            var name when name.Contains("UpdateAppUser") => ("user_updated", "User", true),
            var name when name.Contains("DeleteAppUser") => ("user_deleted", "User", true),
            _ => (string.Empty, string.Empty, false)
        };
    }

    private (int? EntityId, string Title) ExtractEntityInfo(TRequest request, string requestName)
    {
        var props = typeof(TRequest).GetProperties();
        int? entityId = null;
        string title = $"Action: {requestName}";

        // Try to extract Id
        var idProp = props.FirstOrDefault(p => p.Name == "Id");
        if (idProp != null && idProp.PropertyType == typeof(int))
        {
            entityId = (int?)idProp.GetValue(request);
        }

        // Try to extract Title or Name
        var titleProp = props.FirstOrDefault(p => p.Name == "Title" || p.Name == "Name");
        if (titleProp != null && titleProp.PropertyType == typeof(string))
        {
            var extractedTitle = titleProp.GetValue(request) as string;
            if (!string.IsNullOrWhiteSpace(extractedTitle))
            {
                var action = requestName.Contains("Create") ? "oluşturuldu" : 
                            requestName.Contains("Update") ? "güncellendi" : 
                            requestName.Contains("Delete") ? "silindi" : "işlendi";
                title = $"\"{extractedTitle}\" {action}";
            }
        }

        return (entityId, title);
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
}
