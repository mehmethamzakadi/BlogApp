using BlogApp.Application.Behaviors.Transaction;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<IDataResult<LoginResponse>>, ITransactionalRequest;
