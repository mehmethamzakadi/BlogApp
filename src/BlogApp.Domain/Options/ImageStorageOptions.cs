using System.ComponentModel.DataAnnotations;

namespace BlogApp.Domain.Options;

public sealed class ImageStorageOptions
{
    public const string SectionName = "ImageStorage";

    [Required]
    public string RootPath { get; set; } = "wwwroot/uploads";

    [Required]
    public string RequestPath { get; set; } = "/uploads";

    [Range(1, 128)]
    public int MaxFileSizeMb { get; set; } = 5;

    public string DefaultScope { get; set; } = "general";

    [MinLength(1)]
    public string[] AllowedExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".webp"];

    [MinLength(1)]
    public string[] AllowedContentTypes { get; set; } = ["image/jpeg", "image/png", "image/webp"];

    [Range(64, 8000)]
    public int? DefaultMaxWidth { get; set; } = 1600;

    [Range(64, 8000)]
    public int? DefaultMaxHeight { get; set; } = 1600;
}
