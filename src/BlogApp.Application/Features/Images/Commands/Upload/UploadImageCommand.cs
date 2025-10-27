using BlogApp.Application.Abstractions.Images;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Images.Commands.Upload;

public sealed record UploadImageCommand(
    byte[] Content,
    string FileName,
    string ContentType,
    long FileSize,
    string Scope,
    ImageResizeMode ResizeMode,
    int? TargetWidth,
    int? TargetHeight,
    string? Title
) : IRequest<IDataResult<UploadImageResponse>>;
