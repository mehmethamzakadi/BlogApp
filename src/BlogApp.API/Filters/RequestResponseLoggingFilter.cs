using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace BlogApp.API.Filters;

/// <summary>
/// HTTP isteklerini ve yanıtlarını detaylı bilgilerle birlikte loglar
/// </summary>
public class RequestResponseLoggingFilter : IAsyncActionFilter
{
    private readonly ILogger<RequestResponseLoggingFilter> _logger;

    public RequestResponseLoggingFilter(ILogger<RequestResponseLoggingFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;
        var stopwatch = Stopwatch.StartNew();

        // İstek bilgilerini logla
        _logger.LogInformation(
            "HTTP {Method} {Path} başladı. Kullanıcı: {User}, RemoteIp: {RemoteIp}",
            request.Method,
            request.Path,
            context.HttpContext.User?.Identity?.Name ?? "Anonymous",
            context.HttpContext.Connection.RemoteIpAddress?.ToString()
        );

        // POST/PUT istekleri için request body'yi logla (hassas veriler için dikkatli olunmalı)
        if ((request.Method == "POST" || request.Method == "PUT") && context.ActionArguments.Count > 0)
        {
            _logger.LogDebug(
                "İstek gövdesi: {@Arguments}",
                context.ActionArguments
            );
        }

        var executedContext = await next();
        stopwatch.Stop();

        // Yanıt bilgilerini logla
        var statusCode = context.HttpContext.Response.StatusCode;
        var logLevel = statusCode >= 500 ? LogLevel.Error :
                      statusCode >= 400 ? LogLevel.Warning :
                      LogLevel.Information;

        _logger.Log(
            logLevel,
            "HTTP {Method} {Path} tamamlandı. Durum: {StatusCode}, Süre: {ElapsedMilliseconds}ms",
            request.Method,
            request.Path,
            statusCode,
            stopwatch.ElapsedMilliseconds
        );

        // Hataları logla
        if (executedContext.Exception != null)
        {
            _logger.LogError(
                executedContext.Exception,
                "HTTP {Method} {Path} hata ile sonuçlandı",
                request.Method,
                request.Path
            );
        }
    }
}
