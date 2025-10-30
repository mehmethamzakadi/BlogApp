using BlogApp.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController(IMediator mediator) : ControllerBase
{
    protected IMediator Mediator { get; } = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [NonAction]
    protected IActionResult ToResponse<T>(IDataResult<T> result) =>
        StatusCode(result.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest, 
            new ApiResult<T>
            {
                Success = result.Success,
                Message = result.Message,
                Data = result.Data,
                Errors = result.Errors?.ToList() ?? []
            });

    [NonAction]
    protected IActionResult ToResponse(IResult result) =>
        StatusCode(result.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
            new ApiResult<object>
            {
                Success = result.Success,
                Message = result.Message,
                Errors = result.Errors?.ToList() ?? []
            });

    [NonAction]
    protected IActionResult Created<T>(T data, string message = "Kayıt başarıyla oluşturuldu.") =>
        StatusCode(StatusCodes.Status201Created, new ApiResult<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = []
        });
}
