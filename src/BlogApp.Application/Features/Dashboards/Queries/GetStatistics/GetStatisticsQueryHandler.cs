using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Dashboards.Queries.GetStatistics;

/// <summary>
/// Handler for getting dashboard statistics
/// </summary>
public sealed class GetStatisticsQueryHandler(
    IPostRepository postRepository,
    ICategoryRepository categoryRepository)
    : IRequestHandler<GetStatisticsQuery, GetStatisticsResponse>
{
    public async Task<GetStatisticsResponse> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var last7Days = now.AddDays(-7);
        var last30Days = now.AddDays(-30);

        // ✅ FIXED: Using repository-specific methods instead of Query() leak
        var totalPosts = await postRepository.CountAsync(cancellationToken);
        var publishedPosts = await postRepository.CountPublishedAsync(cancellationToken);
        var draftPosts = totalPosts - publishedPosts;
        var postsLast7Days = await postRepository.CountCreatedAfterAsync(last7Days, cancellationToken);
        var postsLast30Days = await postRepository.CountCreatedAfterAsync(last30Days, cancellationToken);

        var totalCategories = await categoryRepository.CountAsync(cancellationToken);

        return new GetStatisticsResponse
        {
            TotalPosts = totalPosts,
            PublishedPosts = publishedPosts,
            DraftPosts = draftPosts,
            TotalCategories = totalCategories,
            PostsLast7Days = postsLast7Days,
            PostsLast30Days = postsLast30Days
        };
    }
}
