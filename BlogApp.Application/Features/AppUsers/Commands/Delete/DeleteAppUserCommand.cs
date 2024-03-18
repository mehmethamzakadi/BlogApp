using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.Delete;

public sealed record DeleteAppUserCommand(int Id) : IRequest<IResult>;
