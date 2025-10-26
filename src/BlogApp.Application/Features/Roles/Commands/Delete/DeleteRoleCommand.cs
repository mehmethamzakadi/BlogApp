using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Roles.Commands.Delete;

public sealed record DeleteRoleCommand(Guid Id) : IRequest<IResult>;
