using BlogApp.Domain.Common;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Events.CategoryEvents;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Categories.EventHandlers;

public class CategoryCreatedEventHandler : INotificationHandler<CategoryCreatedEvent>
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryCreatedEventHandler(
        IActivityLogRepository activityLogRepository,
        IUnitOfWork unitOfWork)
    {
        _activityLogRepository = activityLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CategoryCreatedEvent notification, CancellationToken cancellationToken)
    {
        var activityLog = new ActivityLog
        {
            ActivityType = "category_created",
            EntityType = "Category",
            EntityId = notification.CategoryId,
            Title = $"\"{notification.Name}\" kategorisi oluşturuldu",
            UserId = notification.CreatedById,
            Timestamp = DateTime.UtcNow
        };

        await _activityLogRepository.AddAsync(activityLog);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
