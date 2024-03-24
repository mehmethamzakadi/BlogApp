using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.PasswordReset;

public sealed record PasswordResetCommand(string Email) : IRequest<IResult>;

