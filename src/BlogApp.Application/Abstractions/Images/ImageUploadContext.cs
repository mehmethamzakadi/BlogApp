using System.IO;

namespace BlogApp.Application.Abstractions.Images;

public sealed class ImageUploadContext
{
    public required Stream Content { get; init; }

    public required string FileName { get; init; }

    public required string ContentType { get; init; }

    public required long FileSize { get; init; }

    public string Scope { get; init; } = string.Empty;

    public ImageResizeOptions? Resize { get; init; }
}
