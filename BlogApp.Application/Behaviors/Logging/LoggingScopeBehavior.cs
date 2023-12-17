using MediatR;
using Serilog;

namespace BlogApp.Application.Behaviors.Logging
{
    public class LoggingScopeBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information($"Starting request {typeof(TRequest).Name}");
                var result = await next();
                Log.Information($"Competed request {typeof(TRequest).Name}");

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error request {typeof(TRequest).Name}, Message:{ex.Message}");
                throw;
            }
        }
    }
}
