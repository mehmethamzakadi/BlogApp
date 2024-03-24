using MediatR;

namespace BlogApp.Application.Features.AppRoles.Queries.GetRoleById;

public sealed record GetRoleByIdQueryRequest(int Id) : IRequest<GetRoleByIdQueryResponse>;