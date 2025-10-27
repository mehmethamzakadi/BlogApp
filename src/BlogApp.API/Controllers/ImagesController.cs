using System.IO;
using BlogApp.API.Contracts.Images;
using BlogApp.Application.Features.Images.Commands.Upload;
using BlogApp.Domain.Constants;
using BlogApp.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.API.Controllers;

public sealed class ImagesController(IMediator mediator) : BaseApiController(mediator)
{
    [HttpPost]
    [HasPermission(Permissions.MediaUpload)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] UploadImageRequest request, CancellationToken cancellationToken)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest("Geçerli bir dosya seçiniz.");
        }

        await using var memoryStream = new MemoryStream();
        await request.File.CopyToAsync(memoryStream, cancellationToken);

        var scope = string.IsNullOrWhiteSpace(request.Scope) ? string.Empty : request.Scope.Trim();

        var command = new UploadImageCommand(
            Content: memoryStream.ToArray(),
            FileName: request.File.FileName,
            ContentType: request.File.ContentType ?? string.Empty,
            FileSize: request.File.Length,
            Scope: scope,
            ResizeMode: request.ResizeMode,
            TargetWidth: request.MaxWidth,
            TargetHeight: request.MaxHeight,
            Title: request.Title);

        var result = await Mediator.Send(command, cancellationToken);
        return GetResponseOnlyResultData(result);
    }
}
