using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Users.Commands.Update;

public sealed record UpdateUserCommand(int Id, string UserName, string Email) : IRequest<IResult>;
