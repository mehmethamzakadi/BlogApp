using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Queries.GetRoleById;

public class GetRoleByIdQueryHandler(IRoleService roleService) : IRequestHandler<GetRoleByIdRequest, IDataResult<GetRoleByIdQueryResponse>>
{
    public async Task<IDataResult<GetRoleByIdQueryResponse>> Handle(GetRoleByIdRequest request, CancellationToken cancellationToken)
    {
        AppRole? role = roleService.GetRoleById(request.Id);
        if (role is null)
            return new ErrorDataResult<GetRoleByIdQueryResponse>("Kullanýcý bulunamadý!");

        GetRoleByIdQueryResponse result = new(Id: role.Id, Name: role.Name!);

        return new SuccessDataResult<GetRoleByIdQueryResponse>(result);
    }
}