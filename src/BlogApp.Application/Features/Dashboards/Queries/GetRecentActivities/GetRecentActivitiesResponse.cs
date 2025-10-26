namespace BlogApp.Application.Features.Dashboards.Queries.GetRecentActivities;

public sealed record GetRecentActivitiesResponse
{
    public List<ActivityDto> Activities { get; set; } = new();
}

public sealed record ActivityDto
{
    public Guid Id { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? UserName { get; set; }
}
