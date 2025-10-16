using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.Create;

public sealed record CreateAppUserCommand(string UserName, string Email, string Password) : IRequest<IResult>;
