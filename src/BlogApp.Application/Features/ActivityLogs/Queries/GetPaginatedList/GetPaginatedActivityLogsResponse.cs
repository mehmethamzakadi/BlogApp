namespace BlogApp.Application.Features.ActivityLogs.Queries.GetPaginatedList;

public class GetPaginatedActivityLogsResponse
{
    public Guid Id { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Details { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime Timestamp { get; set; }
}
