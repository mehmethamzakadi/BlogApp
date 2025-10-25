using BlogApp.Domain.Common;
using MediatR;

namespace BlogApp.Application.Behaviors;

/// <summary>
/// Dispatches domain events after the command handler completes successfully
/// This ensures domain events are published AFTER the main transaction commits
/// </summary>
public class DomainEventDispatcherBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public DomainEventDispatcherBehavior(IMediator mediator, IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Execute the handler first
        var response = await next();

        // After successful execution, dispatch domain events
        await DispatchDomainEventsAsync(cancellationToken);

        return response;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEvents = _unitOfWork.GetDomainEvents();

        if (!domainEvents.Any())
            return;

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}
