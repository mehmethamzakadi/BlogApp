using BlogApp.Application.Features.Auths.Login;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Auths.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<IDataResult<LoginResponse>>;
