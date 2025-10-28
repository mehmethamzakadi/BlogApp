using BlogApp.Application.Abstractions;

namespace BlogApp.Infrastructure.Services;

public sealed class CurrentUserService(IExecutionContextAccessor executionContextAccessor) : ICurrentUserService
{
    public Guid? GetCurrentUserId()
    {
        return executionContextAccessor.GetCurrentUserId();
    }
}
