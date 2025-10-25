using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Users.Commands.Create;

public sealed record CreateUserCommand(string UserName, string Email, string Password) : IRequest<IResult>;
