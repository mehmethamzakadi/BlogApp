using System.IO;

namespace BlogApp.Application.Abstractions.Images;

public interface IImageStorageService
{
    Task<ImageUploadResult> UploadAsync(ImageUploadContext context, CancellationToken cancellationToken = default);

    Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default);
}
