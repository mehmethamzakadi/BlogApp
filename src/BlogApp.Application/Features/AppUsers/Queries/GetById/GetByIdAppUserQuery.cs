using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Queries.GetById;

public sealed record GetByIdAppUserQuery(int Id) : IRequest<IDataResult<GetByIdAppUserResponse>>;