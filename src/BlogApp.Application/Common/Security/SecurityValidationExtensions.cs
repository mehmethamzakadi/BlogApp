using FluentValidation;

namespace BlogApp.Application.Common.Security;

/// <summary>
/// FluentValidation extensions for security validation rules.
/// </summary>
public static class SecurityValidationExtensions
{
    /// <summary>
    /// Validates that the string contains no HTML or dangerous scripts.
    /// Use for plain text fields like titles, names, summaries.
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBePlainText<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string? errorMessage = null)
    {
        return ruleBuilder
            .Must(ContentSanitizer.IsPlainTextSafe)
            .WithMessage(errorMessage ?? "Bu alan HTML veya script içeremez!");
    }

    /// <summary>
    /// Validates that HTML content is safe (allows safe HTML tags).
    /// Use for content fields that should contain HTML (like blog post body).
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBeSafeHtml<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string? errorMessage = null)
    {
        return ruleBuilder
            .Must(ContentSanitizer.IsHtmlSafe)
            .WithMessage(errorMessage ?? "İçerik tehlikeli script veya kod içeriyor!");
    }

    /// <summary>
    /// Validates that a URL is safe (no javascript: or data: schemes).
    /// </summary>
    public static IRuleBuilderOptions<T, string> MustBeSafeUrl<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string? errorMessage = null)
    {
        return ruleBuilder
            .Must(url =>
            {
                if (string.IsNullOrWhiteSpace(url))
                    return true;

                var lowerUrl = url.ToLowerInvariant().Trim();
                
                // Only allow http, https schemes
                if (!lowerUrl.StartsWith("http://") && 
                    !lowerUrl.StartsWith("https://") &&
                    !lowerUrl.StartsWith("/")) // Allow relative URLs
                {
                    // Check for dangerous schemes
                    if (lowerUrl.Contains("javascript:") ||
                        lowerUrl.Contains("data:") ||
                        lowerUrl.Contains("vbscript:"))
                    {
                        return false;
                    }
                }
                
                return true;
            })
            .WithMessage(errorMessage ?? "Geçersiz veya güvensiz URL formatı!");
    }
}
