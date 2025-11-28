using Ganss.Xss;

namespace BlogApp.Application.Common.Security;

/// <summary>
/// Provides content sanitization services to prevent XSS attacks.
/// Uses whitelist-based approach with HtmlSanitizer library.
/// </summary>
public static class ContentSanitizer
{
    private static readonly Lazy<HtmlSanitizer> _htmlSanitizer = new(CreateHtmlSanitizer);
    private static readonly Lazy<HtmlSanitizer> _strictSanitizer = new(CreateStrictSanitizer);

    /// <summary>
    /// Creates an HtmlSanitizer configured for blog content (allows safe HTML tags).
    /// </summary>
    private static HtmlSanitizer CreateHtmlSanitizer()
    {
        var sanitizer = new HtmlSanitizer();
        
        // Allowed tags for blog content
        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedTags.Add("p");
        sanitizer.AllowedTags.Add("br");
        sanitizer.AllowedTags.Add("strong");
        sanitizer.AllowedTags.Add("b");
        sanitizer.AllowedTags.Add("em");
        sanitizer.AllowedTags.Add("i");
        sanitizer.AllowedTags.Add("u");
        sanitizer.AllowedTags.Add("s");
        sanitizer.AllowedTags.Add("strike");
        sanitizer.AllowedTags.Add("h1");
        sanitizer.AllowedTags.Add("h2");
        sanitizer.AllowedTags.Add("h3");
        sanitizer.AllowedTags.Add("h4");
        sanitizer.AllowedTags.Add("h5");
        sanitizer.AllowedTags.Add("h6");
        sanitizer.AllowedTags.Add("ul");
        sanitizer.AllowedTags.Add("ol");
        sanitizer.AllowedTags.Add("li");
        sanitizer.AllowedTags.Add("a");
        sanitizer.AllowedTags.Add("img");
        sanitizer.AllowedTags.Add("blockquote");
        sanitizer.AllowedTags.Add("pre");
        sanitizer.AllowedTags.Add("code");
        sanitizer.AllowedTags.Add("table");
        sanitizer.AllowedTags.Add("thead");
        sanitizer.AllowedTags.Add("tbody");
        sanitizer.AllowedTags.Add("tr");
        sanitizer.AllowedTags.Add("th");
        sanitizer.AllowedTags.Add("td");
        sanitizer.AllowedTags.Add("div");
        sanitizer.AllowedTags.Add("span");
        sanitizer.AllowedTags.Add("hr");
        
        // Allowed attributes
        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedAttributes.Add("href");
        sanitizer.AllowedAttributes.Add("src");
        sanitizer.AllowedAttributes.Add("alt");
        sanitizer.AllowedAttributes.Add("title");
        sanitizer.AllowedAttributes.Add("class");
        sanitizer.AllowedAttributes.Add("id");
        sanitizer.AllowedAttributes.Add("target");
        sanitizer.AllowedAttributes.Add("rel");
        sanitizer.AllowedAttributes.Add("width");
        sanitizer.AllowedAttributes.Add("height");
        
        // Allowed URL schemes
        sanitizer.AllowedSchemes.Clear();
        sanitizer.AllowedSchemes.Add("http");
        sanitizer.AllowedSchemes.Add("https");
        sanitizer.AllowedSchemes.Add("mailto");
        
        // Force target="_blank" links to have rel="noopener noreferrer"
        sanitizer.PostProcessNode += (s, e) =>
        {
            if (e.Node is AngleSharp.Html.Dom.IHtmlAnchorElement anchor)
            {
                if (anchor.Target == "_blank")
                {
                    anchor.SetAttribute("rel", "noopener noreferrer");
                }
            }
        };
        
        return sanitizer;
    }

    /// <summary>
    /// Creates a strict sanitizer that removes ALL HTML (for plain text fields).
    /// </summary>
    private static HtmlSanitizer CreateStrictSanitizer()
    {
        var sanitizer = new HtmlSanitizer();
        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedAttributes.Clear();
        sanitizer.AllowedSchemes.Clear();
        return sanitizer;
    }

    /// <summary>
    /// Sanitizes HTML content, allowing safe tags for blog posts.
    /// Use this for body/content fields that should contain HTML.
    /// </summary>
    /// <param name="html">The HTML content to sanitize.</param>
    /// <returns>Sanitized HTML content.</returns>
    public static string SanitizeHtml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;
            
        return _htmlSanitizer.Value.Sanitize(html);
    }

    /// <summary>
    /// Sanitizes text by removing ALL HTML tags.
    /// Use this for plain text fields like titles, names, summaries.
    /// </summary>
    /// <param name="text">The text to sanitize.</param>
    /// <returns>Plain text without any HTML.</returns>
    public static string SanitizePlainText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;
            
        return _strictSanitizer.Value.Sanitize(text);
    }

    /// <summary>
    /// Validates that text contains no dangerous content (for validation purposes).
    /// Returns true if the text is safe.
    /// </summary>
    /// <param name="text">The text to validate.</param>
    /// <returns>True if safe, false if potentially dangerous.</returns>
    public static bool IsPlainTextSafe(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return true;
            
        // Check if sanitized version equals original (no dangerous content removed)
        var sanitized = SanitizePlainText(text);
        return sanitized.Equals(text, StringComparison.Ordinal);
    }

    /// <summary>
    /// Validates that HTML content is safe (no dangerous scripts after sanitization).
    /// Returns true if the content is safe.
    /// </summary>
    /// <param name="html">The HTML to validate.</param>
    /// <returns>True if safe, false if potentially dangerous.</returns>
    public static bool IsHtmlSafe(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return true;
            
        // Sanitize and check if dangerous content was removed
        var sanitized = SanitizeHtml(html);
        
        // If the sanitized version is significantly shorter, dangerous content was removed
        // Allow some tolerance for whitespace normalization
        return sanitized.Length >= html.Length * 0.9;
    }
}
