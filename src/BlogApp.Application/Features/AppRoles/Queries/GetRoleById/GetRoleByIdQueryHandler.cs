using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Queries.GetRoleById;

public sealed class GetRoleByIdQueryHandler(IRoleService roleService) : IRequestHandler<GetRoleByIdRequest, IDataResult<GetRoleByIdQueryResponse>>
{
    public Task<IDataResult<GetRoleByIdQueryResponse>> Handle(GetRoleByIdRequest request, CancellationToken cancellationToken)
    {
        AppRole? role = roleService.GetRoleById(request.Id);
        if (role is null)
        {
            IDataResult<GetRoleByIdQueryResponse> errorResult = new ErrorDataResult<GetRoleByIdQueryResponse>("Kullanıcı bulunamadı!");
            return Task.FromResult(errorResult);
        }

        GetRoleByIdQueryResponse result = new(Id: role.Id, Name: role.Name!);
        IDataResult<GetRoleByIdQueryResponse> successResult = new SuccessDataResult<GetRoleByIdQueryResponse>(result);
        return Task.FromResult(successResult);
    }
}
