using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Roles.Queries.GetRoleById;

public sealed record GetRoleByIdRequest(Guid Id) : IRequest<IDataResult<GetRoleByIdQueryResponse>>;
