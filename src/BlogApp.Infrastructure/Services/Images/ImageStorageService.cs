using BlogApp.Application.Abstractions.Images;
using BlogApp.Infrastructure.Options;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BlogApp.Infrastructure.Services.Images;

public sealed class ImageStorageService : IImageStorageService
{
    private readonly ImageStorageOptions _options;

    public ImageStorageService(IOptions<ImageStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<ImageUploadResult> UploadAsync(ImageUploadContext context, CancellationToken cancellationToken = default)
    {
        var rootPath = Path.GetFullPath(_options.RootPath);
        Directory.CreateDirectory(rootPath);

        var scopePath = Path.Combine(rootPath, context.Scope ?? _options.DefaultScope);
        Directory.CreateDirectory(scopePath);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(context.FileName)}";
        var filePath = Path.Combine(scopePath, fileName);

        using var image = await Image.LoadAsync(context.Content, cancellationToken);
        
        if (_options.DefaultMaxWidth.HasValue || _options.DefaultMaxHeight.HasValue)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(_options.DefaultMaxWidth ?? image.Width, _options.DefaultMaxHeight ?? image.Height),
                Mode = ResizeMode.Max
            }));
        }

        await image.SaveAsync(filePath, cancellationToken);

        var relativePath = Path.Combine(context.Scope ?? _options.DefaultScope, fileName);
        var requestPath = $"{_options.RequestPath.TrimEnd('/')}/{relativePath.Replace("\\", "/")}";

        return new ImageUploadResult
        {
            StoredFileName = fileName,
            RelativePath = relativePath,
            RelativeUrl = requestPath,
            ContentType = context.ContentType,
            FileSize = context.FileSize,
            Width = image.Width,
            Height = image.Height
        };
    }

    public Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(Path.GetFullPath(_options.RootPath), relativePath);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
        return Task.CompletedTask;
    }
}
