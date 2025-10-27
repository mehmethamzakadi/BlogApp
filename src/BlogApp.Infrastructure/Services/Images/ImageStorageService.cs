using System.Security.Cryptography;
using System.Text.RegularExpressions;
using BlogApp.Application.Abstractions.Images;
using BlogApp.Domain.Exceptions;
using BlogApp.Domain.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;

namespace BlogApp.Infrastructure.Services.Images;

public sealed class ImageStorageService : IImageStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ImageStorageService> _logger;
    private readonly ImageStorageOptions _options;
    private readonly HashSet<string> _allowedExtensions;
    private readonly HashSet<string> _allowedContentTypes;
    private readonly long _maxFileSizeBytes;
    private readonly string _physicalRootPath;
    private readonly string _requestPath;

    public ImageStorageService(
        IWebHostEnvironment environment,
        IOptions<ImageStorageOptions> options,
        ILogger<ImageStorageService> logger)
    {
        _environment = environment;
        _logger = logger;
        _options = options.Value;

        _allowedExtensions = new HashSet<string>(_options.AllowedExtensions.Select(x => NormalizeExtension(x)), StringComparer.OrdinalIgnoreCase);
        _allowedContentTypes = new HashSet<string>(_options.AllowedContentTypes.Select(x => x.Trim()), StringComparer.OrdinalIgnoreCase);
        _maxFileSizeBytes = Math.Max(1, _options.MaxFileSizeMb) * 1024L * 1024L;

        _physicalRootPath = ResolveRootPath(_options.RootPath);
        _requestPath = NormalizeRequestPath(_options.RequestPath);

        Directory.CreateDirectory(_physicalRootPath);
    }

    public async Task<ImageUploadResult> UploadAsync(ImageUploadContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(context.Content);

        if (context.FileSize <= 0)
        {
            throw new ImageStorageException("Yüklenecek dosya içeriği boş.");
        }

        if (context.FileSize > _maxFileSizeBytes)
        {
            throw new ImageStorageException($"Dosya boyutu {_options.MaxFileSizeMb} MB sınırını aşamaz.");
        }

        var extension = NormalizeExtension(Path.GetExtension(context.FileName));
        if (!_allowedExtensions.Contains(extension))
        {
            throw new ImageStorageException($"Desteklenmeyen dosya uzantısı: {extension}");
        }

        if (!string.IsNullOrWhiteSpace(context.ContentType) && !_allowedContentTypes.Contains(context.ContentType.Trim()))
        {
            throw new ImageStorageException($"Desteklenmeyen içerik tipi: {context.ContentType}");
        }

        await using var workingStream = await PrepareStreamAsync(context.Content, cancellationToken);

        // Format tespiti
        IImageFormat? format = await Image.DetectFormatAsync(workingStream, cancellationToken);
        if (format is null)
        {
            throw new ImageStorageException("Yüklenen dosya geçerli bir görsel değil.");
        }

        if (!_allowedContentTypes.Contains(format.DefaultMimeType))
        {
            throw new ImageStorageException($"Desteklenmeyen görsel formatı: {format.Name}");
        }

        // Uzantıyı gerçek formata göre güncelle
        var detectedExtension = NormalizeExtension($".{format.FileExtensions.FirstOrDefault() ?? extension.TrimStart('.')}");
        if (!_allowedExtensions.Contains(detectedExtension))
        {
            throw new ImageStorageException($"Desteklenmeyen görsel uzantısı: {detectedExtension}");
        }

        workingStream.Position = 0;
        using Image image = await Image.LoadAsync(workingStream, cancellationToken);

        ResizeImageIfNeeded(image, context.Resize);

        var scope = SanitizeScope(context.Scope);
        var now = DateTime.UtcNow;
        var relativeDirectory = Path.Combine(scope, now.Year.ToString("0000"), now.Month.ToString("00"));
        var targetDirectory = Path.Combine(_physicalRootPath, relativeDirectory);
        Directory.CreateDirectory(targetDirectory);

        var hashedFileName = GenerateFileName(detectedExtension);
        var filePath = Path.Combine(targetDirectory, hashedFileName);

        await using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await image.SaveAsync(fileStream, format, cancellationToken);
        }

        var relativePath = Path.Combine(relativeDirectory, hashedFileName).Replace(Path.DirectorySeparatorChar, '/');
        var relativeUrl = BuildRelativeUrl(relativePath);

        return new ImageUploadResult
        {
            StoredFileName = hashedFileName,
            RelativePath = relativePath,
            RelativeUrl = relativeUrl,
            ContentType = format.DefaultMimeType,
            FileSize = new FileInfo(filePath).Length,
            Width = image.Width,
            Height = image.Height
        };
    }

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return Task.CompletedTask;
        }

        var sanitized = SanitizeRelativePath(relativePath);
        if (string.IsNullOrWhiteSpace(sanitized))
        {
            return Task.CompletedTask;
        }

        var fullPath = Path.Combine(_physicalRootPath, sanitized.Replace('/', Path.DirectorySeparatorChar));
        var safeFullPath = Path.GetFullPath(fullPath);

        if (!safeFullPath.StartsWith(_physicalRootPath, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Silinmek istenen dosya kök klasör dışında: {Path}", relativePath);
            return Task.CompletedTask;
        }

        if (!File.Exists(safeFullPath))
        {
            return Task.CompletedTask;
        }

        try
        {
            File.Delete(safeFullPath);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Görsel dosyası silinirken hata oluştu: {Path}", relativePath);
        }

        return Task.CompletedTask;
    }

    private void ResizeImageIfNeeded(Image image, ImageResizeOptions? resizeOptions)
    {
        var options = resizeOptions ?? BuildDefaultResizeOptions();
        if (options is null)
        {
            return;
        }

        var targetWidth = options.Width ?? _options.DefaultMaxWidth;
        var targetHeight = options.Height ?? _options.DefaultMaxHeight;

        if ((targetWidth is null || targetWidth <= 0) && (targetHeight is null || targetHeight <= 0))
        {
            return;
        }

        var desiredWidth = targetWidth ?? (targetHeight.HasValue
            ? (int)Math.Round(image.Width * (targetHeight.Value / (double)image.Height))
            : image.Width);

        var desiredHeight = targetHeight ?? (targetWidth.HasValue
            ? (int)Math.Round(image.Height * (targetWidth.Value / (double)image.Width))
            : image.Height);

        var resize = new ResizeOptions
        {
            Mode = options.Mode switch
            {
                ImageResizeMode.Crop => ResizeMode.Crop,
                _ => ResizeMode.Max
            },
            Size = new Size(Math.Max(1, desiredWidth), Math.Max(1, desiredHeight)),
            Position = AnchorPositionMode.Center
        };

        image.Mutate(x => x.Resize(resize));
    }

    private ImageResizeOptions? BuildDefaultResizeOptions()
    {
        if (_options.DefaultMaxWidth is null && _options.DefaultMaxHeight is null)
        {
            return null;
        }

        return new ImageResizeOptions
        {
            Width = _options.DefaultMaxWidth,
            Height = _options.DefaultMaxHeight,
            Mode = ImageResizeMode.Fit
        };
    }

    private static async Task<Stream> PrepareStreamAsync(Stream source, CancellationToken cancellationToken)
    {
        if (source is MemoryStream memoryStream && memoryStream.CanSeek)
        {
            memoryStream.Position = 0;
            return memoryStream;
        }

        if (source.CanSeek)
        {
            source.Position = 0;
        }

        var copy = new MemoryStream();
        await source.CopyToAsync(copy, cancellationToken);
        copy.Position = 0;
        return copy;
    }

    private static string GenerateFileName(string extension)
    {
        Span<byte> buffer = stackalloc byte[32];
        RandomNumberGenerator.Fill(buffer);
        return $"{Convert.ToHexString(buffer).ToLowerInvariant()}{extension}";
    }

    private string ResolveRootPath(string configuredPath)
    {
        var path = string.IsNullOrWhiteSpace(configuredPath)
            ? Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads")
            : configuredPath;

        if (!Path.IsPathRooted(path))
        {
            path = Path.Combine(_environment.ContentRootPath, path);
        }

        return Path.GetFullPath(path);
    }

    private static string NormalizeExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return string.Empty;
        }

        extension = extension.Trim();
        return extension.StartsWith('.') ? extension.ToLowerInvariant() : $".{extension.ToLowerInvariant()}";
    }

    private string SanitizeScope(string scope)
    {
        var sanitized = string.IsNullOrWhiteSpace(scope) ? _options.DefaultScope : scope.Trim().ToLowerInvariant();
        sanitized = Regex.Replace(sanitized, "[^a-z0-9-_]", string.Empty);
        return string.IsNullOrWhiteSpace(sanitized) ? _options.DefaultScope : sanitized;
    }

    private string BuildRelativeUrl(string relativePath)
    {
        var trimmed = relativePath.TrimStart('/', '\\');
        var combined = string.IsNullOrEmpty(_requestPath) ? $"/{trimmed}" : $"{_requestPath}/{trimmed}";
        combined = combined.Replace("\\", "/");
        return combined.StartsWith('/') ? combined : $"/{combined}";
    }

    private string SanitizeRelativePath(string path)
    {
        var sanitized = path.Replace(_requestPath, string.Empty, StringComparison.OrdinalIgnoreCase)
            .TrimStart('/', '\\');

        sanitized = sanitized.Replace("..", string.Empty, StringComparison.Ordinal);
        sanitized = sanitized.Replace("\\", "/");
        return sanitized;
    }

    private static string NormalizeRequestPath(string requestPath)
    {
        if (string.IsNullOrWhiteSpace(requestPath))
        {
            return string.Empty;
        }

        var trimmed = requestPath.Trim();
        if (!trimmed.StartsWith('/'))
        {
            trimmed = "/" + trimmed;
        }

        return trimmed.TrimEnd('/');
    }
}
