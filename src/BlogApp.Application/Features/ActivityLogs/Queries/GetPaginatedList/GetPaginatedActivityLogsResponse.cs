using BlogApp.Domain.Common.Responses;

namespace BlogApp.Application.Features.ActivityLogs.Queries.GetPaginatedList;

public class GetPaginatedActivityLogsResponse
{
    public int Id { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Details { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime Timestamp { get; set; }
}
