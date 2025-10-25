namespace BlogApp.Application.Features.Dashboards.Queries.GetStatistics;

public sealed record GetStatisticsResponse
{
    public int TotalPosts { get; set; }
    public int PublishedPosts { get; set; }
    public int DraftPosts { get; set; }
    public int TotalCategories { get; set; }
    public int PostsLast7Days { get; set; }
    public int PostsLast30Days { get; set; }
}
