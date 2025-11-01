using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlogApp.Application.Behaviors;

/// <summary>
/// Optimistic concurrency exception'ları yakalar ve retry mekanizması sağlar
/// DbUpdateConcurrencyException durumunda otomatik retry yapar
/// </summary>
public sealed class ConcurrencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ConcurrencyBehavior<TRequest, TResponse>> _logger;
    private const int MaxRetries = 3;
    private const int BaseDelayMs = 100;

    public ConcurrencyBehavior(ILogger<ConcurrencyBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var retryCount = 0;

        while (retryCount < MaxRetries)
        {
            try
            {
                return await next();
            }
            catch (DbUpdateConcurrencyException ex) when (retryCount < MaxRetries - 1)
            {
                retryCount++;

                _logger.LogWarning(ex,
                    "Concurrency conflict detected for {RequestType}. Retry {Retry}/{MaxRetries}",
                    typeof(TRequest).Name,
                    retryCount,
                    MaxRetries);

                // Exponential backoff: 100ms, 200ms, 400ms
                var delay = BaseDelayMs * (int)Math.Pow(2, retryCount - 1);
                await Task.Delay(delay, cancellationToken);

                // EF Core entry'leri refresh et
                foreach (var entry in ex.Entries)
                {
                    await entry.ReloadAsync(cancellationToken);
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Son deneme de başarısız oldu
                _logger.LogError(ex,
                    "Concurrency conflict for {RequestType} after {MaxRetries} retries",
                    typeof(TRequest).Name,
                    MaxRetries);

                throw new Domain.Exceptions.ConcurrencyException(
                    "The record you attempted to edit was modified by another user after you got the original value. " +
                    "The edit operation was canceled and the current values in the database have been displayed. " +
                    "If you still want to edit this record, please try again.",
                    ex);
            }
        }

        throw new InvalidOperationException("Unreachable code");
    }
}
