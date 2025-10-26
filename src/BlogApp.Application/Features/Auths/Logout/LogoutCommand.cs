using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Auths.Logout;

public sealed record LogoutCommand(string RefreshToken) : IRequest<IResult>;
