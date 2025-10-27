using System.IO;
using BlogApp.Application.Abstractions;
using BlogApp.Application.Abstractions.Images;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Exceptions;
using BlogApp.Domain.Common;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Images.Commands.Upload;

public sealed class UploadImageCommandHandler(
    IImageStorageService imageStorageService,
    IImageRepository imageRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UploadImageCommand, IDataResult<UploadImageResponse>>
{
    public async Task<IDataResult<UploadImageResponse>> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        if (request.Content.Length == 0)
        {
            return new ErrorDataResult<UploadImageResponse>("Yüklenecek dosya içeriği bulunamadı.");
        }

        try
        {
            await using var contentStream = new MemoryStream(request.Content);

            var uploadContext = new ImageUploadContext
            {
                Content = contentStream,
                FileName = request.FileName,
                ContentType = request.ContentType,
                FileSize = request.FileSize,
                Scope = request.Scope,
                Resize = request.TargetWidth is null && request.TargetHeight is null
                    ? null
                    : new ImageResizeOptions
                    {
                        Width = request.TargetWidth,
                        Height = request.TargetHeight,
                        Mode = request.ResizeMode
                    }
            };

            ImageUploadResult uploadResult = await imageStorageService.UploadAsync(uploadContext, cancellationToken);

            var normalizedTitle = string.IsNullOrWhiteSpace(request.Title) ? null : request.Title.Trim();

            var image = new Image
            {
                Name = uploadResult.StoredFileName,
                Title = normalizedTitle,
                Size = (int)Math.Min(uploadResult.FileSize, int.MaxValue),
                Path = uploadResult.RelativePath,
                Type = uploadResult.ContentType
            };

            await imageRepository.AddAsync(image);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var response = new UploadImageResponse
            {
                ImageId = image.Id,
                FileName = uploadResult.StoredFileName,
                ContentType = uploadResult.ContentType,
                Size = uploadResult.FileSize,
                RelativePath = uploadResult.RelativePath,
                Url = uploadResult.RelativeUrl,
                Width = uploadResult.Width,
                Height = uploadResult.Height
            };

            return new SuccessDataResult<UploadImageResponse>(response, "Görsel yükleme işlemi tamamlandı.");
        }
        catch (ImageStorageException storageException)
        {
            return new ErrorDataResult<UploadImageResponse>(storageException.Message);
        }
    }
}
