using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.Update;

public sealed record UpdateRoleCommand(int Id, string Name) : IRequest<IResult>;