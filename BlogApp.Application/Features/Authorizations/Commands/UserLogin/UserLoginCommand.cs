using BlogApp.Application.Behaviors.Transaction;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Authorizations.Commands.UserLogin;

public sealed record UserLoginCommand(string Email, string Password) : IRequest<IDataResult<TokenResponse>>, ITransactionalRequest;
