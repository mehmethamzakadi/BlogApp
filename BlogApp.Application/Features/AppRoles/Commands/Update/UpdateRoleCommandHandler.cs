using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.Update;

public class UpdateRoleCommandHandler(IRoleService roleService) : IRequestHandler<UpdateRoleCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var checkRole = roleService.AnyRole(request.Name);
        if (checkRole)
            return Result<string>.FailureResult($"Güncellemek istediðiniz {request.Name} rolü sistemde mevcut!");

        var result = await roleService.UpdateRole(new AppRole { Id = request.Id, Name = request.Name });

        return result.Succeeded
            ? Result<string>.SuccessResult("Rol güncellendi.")
            : Result<string>.FailureResult("Ýþlem sýrasýnda bir hata oluþtu");
    }
}