using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Roles.Commands.Update;

public sealed record UpdateRoleCommand(Guid Id, string Name) : IRequest<IResult>;
