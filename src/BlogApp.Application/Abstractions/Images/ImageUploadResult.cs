namespace BlogApp.Application.Abstractions.Images;

public sealed class ImageUploadResult
{
    public required string StoredFileName { get; init; }

    public required string RelativePath { get; init; }

    public required string RelativeUrl { get; init; }

    public required string ContentType { get; init; }

    public required long FileSize { get; init; }

    public required int Width { get; init; }

    public required int Height { get; init; }
}
