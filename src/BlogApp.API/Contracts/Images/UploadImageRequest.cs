using BlogApp.Application.Abstractions.Images;
using System.ComponentModel.DataAnnotations;

#nullable enable

namespace BlogApp.API.Contracts.Images;

public sealed class UploadImageRequest
{
    [Required]
    public IFormFile File { get; set; } = default!;

    [StringLength(50)]
    public string? Scope { get; set; }

    [Range(64, 8000)]
    public int? MaxWidth { get; set; }

    [Range(64, 8000)]
    public int? MaxHeight { get; set; }

    public ImageResizeMode ResizeMode { get; set; } = ImageResizeMode.Fit;

    [StringLength(150)]
    public string? Title { get; set; }
}
