using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Roles.Queries.GetRoleById;

public sealed record GetRoleByIdRequest(int Id) : IRequest<IDataResult<GetRoleByIdQueryResponse>>;
