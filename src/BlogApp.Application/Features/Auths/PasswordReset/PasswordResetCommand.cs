using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Auths.PasswordReset;

public sealed record PasswordResetCommand(string Email) : IRequest<IResult>;

