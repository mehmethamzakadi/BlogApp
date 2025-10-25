using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Roles.Commands.Create;

public sealed record CreateRoleCommand(string Name) : IRequest<IResult>;
