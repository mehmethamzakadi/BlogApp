using System;
using System.Collections.Generic;
using System.Linq;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BlogApp.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception while processing the request");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var apiResult = new ApiResult<object>
            {
                Success = false,
                Message = "İsteğiniz işlenirken bir hata oluştu.",
                InternalMessage = exception.Message,
                Errors = new List<string>()
            };

            response.StatusCode = exception switch
            {
                ValidationException validationException => BuildValidationError(validationException, apiResult),
                BadRequestException => SetApiResult(apiResult, StatusCodes.Status400BadRequest, exception.Message),
                NotFoundException => SetApiResult(apiResult, StatusCodes.Status404NotFound, exception.Message),
                AuthenticationErrorException => SetApiResult(
                    apiResult,
                    StatusCodes.Status401Unauthorized,
                    string.IsNullOrWhiteSpace(exception.Message)
                        ? "Kimlik doğrulama başarısız."
                        : exception.Message),
                PasswordChangeFailedException => SetApiResult(apiResult, StatusCodes.Status400BadRequest, exception.Message),
                InvalidOperationException => SetApiResult(apiResult, StatusCodes.Status400BadRequest, exception.Message),
                ArgumentException => SetApiResult(apiResult, StatusCodes.Status400BadRequest, exception.Message),
                _ => SetApiResult(apiResult, StatusCodes.Status500InternalServerError, "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin.")
            };

            await response.WriteAsJsonAsync(apiResult);
        }

        private static int BuildValidationError(ValidationException validationException, ApiResult<object> apiResult)
        {
            apiResult.Message = validationException.Errors.FirstOrDefault() ?? "Geçersiz veya eksik bilgiler mevcut.";
            apiResult.Errors = validationException.Errors;
            return StatusCodes.Status400BadRequest;
        }

        private static int SetApiResult(ApiResult<object> apiResult, int statusCode, string message)
        {
            apiResult.Message = string.IsNullOrWhiteSpace(message)
                ? "İsteğiniz işlenirken bir hata oluştu."
                : message;
            return statusCode;
        }
    }
}
