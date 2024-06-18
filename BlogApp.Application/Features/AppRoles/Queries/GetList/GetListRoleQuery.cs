using BlogApp.Domain.Common.Requests;
using BlogApp.Domain.Common.Responses;
using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Queries.GetList;

public sealed record GetListRoleQuery(PageRequest PageRequest) : IRequest<Result<GetListResponse<GetListAppRoleResponse>>>;

