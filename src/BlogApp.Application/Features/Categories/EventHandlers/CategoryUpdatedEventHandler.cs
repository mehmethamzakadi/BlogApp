using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.CategoryEvents;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.EventHandlers;

public class CategoryUpdatedEventHandler : INotificationHandler<CategoryUpdatedEvent>
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryUpdatedEventHandler(
        IActivityLogRepository activityLogRepository,
        IUnitOfWork unitOfWork)
    {
        _activityLogRepository = activityLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CategoryUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var activityLog = new ActivityLog
        {
            ActivityType = "category_updated",
            EntityType = "Category",
            EntityId = notification.CategoryId,
            Title = $"\"{notification.Name}\" kategorisi güncellendi",
            UserId = notification.UpdatedById,
            Timestamp = DateTime.UtcNow
        };

        await _activityLogRepository.AddAsync(activityLog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
