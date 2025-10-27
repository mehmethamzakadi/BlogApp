namespace BlogApp.Application.Abstractions.Images;

public sealed class ImageResizeOptions
{
    public int? Width { get; init; }

    public int? Height { get; init; }

    public ImageResizeMode Mode { get; init; } = ImageResizeMode.Fit;
}
