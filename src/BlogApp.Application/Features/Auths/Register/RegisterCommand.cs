using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Auths.Register;

public sealed record RegisterCommand(string UserName, string Email, string Password) : IRequest<IResult>;
