namespace BlogApp.Application.Features.Images.Commands.Upload;

public sealed record UploadImageResponse
{
    public Guid ImageId { get; init; }

    public string FileName { get; init; } = string.Empty;

    public string ContentType { get; init; } = string.Empty;

    public long Size { get; init; }

    public string RelativePath { get; init; } = string.Empty;

    public string Url { get; init; } = string.Empty;

    public int Width { get; init; }

    public int Height { get; init; }
}
