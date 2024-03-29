using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.PasswordVerify;

public sealed record PasswordVerifyCommand(string ResetToken, string UserId) : IRequest<IDataResult<bool>>;
