using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.Delete;


public sealed record DeleteRoleCommand(int Id) : IRequest<IResult>;