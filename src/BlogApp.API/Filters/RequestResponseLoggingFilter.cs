using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace BlogApp.API.Filters;

/// <summary>
/// Logs HTTP requests and responses with detailed information
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

        // Log request
        _logger.LogInformation(
            "HTTP {Method} {Path} started. User: {User}, RemoteIp: {RemoteIp}",
            request.Method,
            request.Path,
            context.HttpContext.User?.Identity?.Name ?? "Anonymous",
            context.HttpContext.Connection.RemoteIpAddress?.ToString()
        );

        // Log request body for POST/PUT (be careful with sensitive data)
        if ((request.Method == "POST" || request.Method == "PUT") && context.ActionArguments.Count > 0)
        {
            _logger.LogDebug(
                "Request body: {@Arguments}",
                context.ActionArguments
            );
        }

        var executedContext = await next();
        stopwatch.Stop();

        // Log response
        var statusCode = context.HttpContext.Response.StatusCode;
        var logLevel = statusCode >= 500 ? LogLevel.Error :
                      statusCode >= 400 ? LogLevel.Warning :
                      LogLevel.Information;

        _logger.Log(
            logLevel,
            "HTTP {Method} {Path} completed with {StatusCode} in {ElapsedMilliseconds}ms",
            request.Method,
            request.Path,
            statusCode,
            stopwatch.ElapsedMilliseconds
        );

        // Log exceptions
        if (executedContext.Exception != null)
        {
            _logger.LogError(
                executedContext.Exception,
                "HTTP {Method} {Path} failed with exception",
                request.Method,
                request.Path
            );
        }
    }
}
