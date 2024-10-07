using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.Update;

public sealed record UpdateAppUserCommand(int Id, string UserName, string Email) : IRequest<IResult>;
