using BlogApp.Domain.Common.Results;
using MediatR;

namespace BlogApp.Application.Features.Permissions.Queries.GetAll;

public record GetAllPermissionsQuery : IRequest<IDataResult<GetAllPermissionsResponse>>;
