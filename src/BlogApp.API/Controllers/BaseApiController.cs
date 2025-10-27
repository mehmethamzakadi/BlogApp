using BlogApp.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController(IMediator mediator) : ControllerBase
    {
        protected IMediator Mediator { get; } = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [NonAction]
        public IActionResult GetResponse<T>(IDataResult<T> result)
        {
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [NonAction]
        public IActionResult GetResponseOnlyResult(IResult result)
        {
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [NonAction]
        public IActionResult GetResponseOnlyResultMessage(IResult result)
        {
            var apiResult = CreateApiResult(result, result.Message);
            var statusCode = result.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;

            return ApiResponse(result.Success, statusCode, apiResult);
        }

        [NonAction]
        public IActionResult GetResponseOnlyResultData<T>(IDataResult<T> result)
        {
            var apiResult = CreateApiResult(result);
            var statusCode = result.Success ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest;

            return ApiResponse(result.Success, statusCode, apiResult);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="internalMessage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult Success<T>(string message, string internalMessage, T data)
        {
            var apiResult = CreateApiResult(true, message, internalMessage, data);

            return ApiResponse(true, StatusCodes.Status200OK, apiResult);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult Success<T>(ApiResult<T> data)
        {
            return ApiResponse(true, StatusCodes.Status200OK, data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="internalMessage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult Created<T>(string message, string internalMessage, T data)
        {
            var apiResult = CreateApiResult(true, message, internalMessage, data);

            return ApiResponse(true, StatusCodes.Status201Created, apiResult);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult Created<T>(ApiResult<T> data)
        {
            return ApiResponse(true, StatusCodes.Status201Created, data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="internalMessage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult NoContent<T>(string message, string internalMessage, T data)
        {
            var apiResult = CreateApiResult(true, message, internalMessage, data);

            return ApiResponse(true, StatusCodes.Status200OK, apiResult);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult NoContent<T>(ApiResult<T> data)
        {
            return ApiResponse(true, StatusCodes.Status200OK, data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="internalMessage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult BadRequest<T>(string message, string internalMessage, T data)
        {
            var apiResult = CreateApiResult(false, message, internalMessage, data);

            return ApiResponse(false, StatusCodes.Status400BadRequest, apiResult);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult BadRequest<T>(ApiResult<T> data)
        {
            return ApiResponse(false, StatusCodes.Status400BadRequest, data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="internalMessage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult Unauthorized<T>(string message, string internalMessage, T data)
        {
            var apiResult = CreateApiResult(false, message, internalMessage, data);

            return ApiResponse(false, StatusCodes.Status401Unauthorized, apiResult);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult Unauthorized<T>(ApiResult<T> data)
        {
            return ApiResponse(false, StatusCodes.Status401Unauthorized, data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="internalMessage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult Forbidden<T>(string message, string internalMessage, T data)
        {
            var apiResult = CreateApiResult(false, message, internalMessage, data);

            return ApiResponse(false, StatusCodes.Status403Forbidden, apiResult);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult Forbidden<T>(ApiResult<T> data)
        {
            return ApiResponse(false, StatusCodes.Status403Forbidden, data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="internalMessage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult NotFound<T>(string message, string internalMessage, T data)
        {
            var apiResult = CreateApiResult(false, message, internalMessage, data);

            return ApiResponse(false, StatusCodes.Status404NotFound, apiResult);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult NotFound<T>(ApiResult<T> data)
        {
            return ApiResponse(false, StatusCodes.Status404NotFound, data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="internalMessage"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult Error<T>(string message, string internalMessage, T data)
        {
            var apiResult = CreateApiResult(false, message, internalMessage, data);

            return ApiResponse(false, StatusCodes.Status500InternalServerError, apiResult);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [NonAction]
        protected IActionResult Error<T>(ApiResult<T> data)
        {
            return ApiResponse(false, StatusCodes.Status500InternalServerError, data);
        }

        private ApiResult<T> CreateApiResult<T>(IDataResult<T> result)
        {
            return CreateApiResult(result.Success, result.Message, string.Empty, result.Data, result.Errors);
        }

        private ApiResult<T> CreateApiResult<T>(IResult result, T data)
        {
            return CreateApiResult(result.Success, result.Message, string.Empty, data, result.Errors);
        }

        private ApiResult<T> CreateApiResult<T>(
            bool success,
            string message,
            string internalMessage,
            T data,
            IEnumerable<string>? errors = null)
        {
            return new ApiResult<T>
            {
                Success = success,
                Message = message,
                InternalMessage = internalMessage,
                Data = data,
                Errors = errors is null ? new List<string>() : new List<string>(errors)
            };
        }

        private IActionResult ApiResponse<T>(bool success, int statusCode, ApiResult<T> result)
        {
            result.Success = success;

            if (result.Errors == null)
            {
                result.Errors = new List<string>();
            }

            return StatusCode(statusCode, result);
        }
    }
}
