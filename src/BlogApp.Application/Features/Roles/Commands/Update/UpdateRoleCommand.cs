using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Roles.Commands.Update;

public sealed record UpdateRoleCommand(int Id, string Name) : IRequest<IResult>;
