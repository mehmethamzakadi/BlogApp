using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.Dashboards.Queries.GetStatistics;

public class GetStatisticsQueryHandler(
    IPostRepository postRepository,
    ICategoryRepository categoryRepository) 
    : IRequestHandler<GetStatisticsQuery, GetStatisticsResponse>
{
    public async Task<GetStatisticsResponse> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var last7Days = now.AddDays(-7);
        var last30Days = now.AddDays(-30);

        var totalPosts = await postRepository.Query().CountAsync(cancellationToken);
        var publishedPosts = await postRepository.Query().CountAsync(p => p.IsPublished, cancellationToken);
        var draftPosts = totalPosts - publishedPosts;
        var postsLast7Days = await postRepository.Query().CountAsync(p => p.CreatedDate >= last7Days, cancellationToken);
        var postsLast30Days = await postRepository.Query().CountAsync(p => p.CreatedDate >= last30Days, cancellationToken);

        var totalCategories = await categoryRepository.Query().CountAsync(cancellationToken);

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
