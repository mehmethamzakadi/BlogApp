using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Users.Queries.GetById;

public sealed record GetByIdUserQuery(Guid Id) : IRequest<IDataResult<GetByIdUserResponse>>;
