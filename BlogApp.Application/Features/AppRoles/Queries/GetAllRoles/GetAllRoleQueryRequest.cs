using MediatR;

namespace BlogApp.Application.Features.AppRoles.Queries.GetAllRoles;

public sealed class GetAllRoleQueryRequest() : IRequest<GetAllRoleQueryResponse>;