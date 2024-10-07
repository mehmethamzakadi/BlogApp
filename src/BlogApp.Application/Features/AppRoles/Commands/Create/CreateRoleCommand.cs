using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.Create;

public sealed record CreateRoleCommand(string Name) : IRequest<IResult>;