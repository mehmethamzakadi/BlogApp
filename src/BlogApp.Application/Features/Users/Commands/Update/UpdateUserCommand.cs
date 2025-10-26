using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Users.Commands.Update;

public sealed record UpdateUserCommand(Guid Id, string UserName, string Email) : IRequest<IResult>;
