using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Users.Commands.Delete;

public sealed record DeleteUserCommand(Guid Id) : IRequest<IResult>;
