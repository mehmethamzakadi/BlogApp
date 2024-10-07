using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Auths.PasswordVerify;

public sealed record PasswordVerifyCommand(string ResetToken, string UserId) : IRequest<IDataResult<bool>>;
