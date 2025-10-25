using MediatR;
using Serilog;

namespace BlogApp.Application.Behaviors
{
    public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                // Yapılandırılmış loglama kullanarak istek başlatıldı
                Log.Information("{RequestType} isteği başlatılıyor", typeof(TRequest).Name);
                var result = await next();
                Log.Information("{RequestType} isteği tamamlandı", typeof(TRequest).Name);

                return result;
            }
            catch (Exception ex)
            {
                // Yapılandırılmış loglama ile hata kaydı
                Log.Error(ex, "{RequestType} isteği sırasında hata oluştu", typeof(TRequest).Name);
                throw;
            }
        }
    }
}
