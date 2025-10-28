using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Auths.Login;

public sealed record LoginCommand(
    string Email,
    string Password,
    string? DeviceId = null) : IRequest<IDataResult<LoginResponse>>;
