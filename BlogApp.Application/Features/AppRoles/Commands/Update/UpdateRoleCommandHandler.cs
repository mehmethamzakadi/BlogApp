using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.Update;

public class UpdateRoleCommandHandler(IRoleService roleService) : IRequestHandler<UpdateRoleCommand, IResult>
{
    public async Task<IResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var checkRole = roleService.AnyRole(request.Name);
        if (checkRole)
            return new ErrorResult($"Güncellemek istediðiniz {request.Name} rolü sistemde mevcut!");

        var result = await roleService.UpdateRole(new AppRole { Id = request.Id, Name = request.Name });

        return result.Succeeded ? new SuccessResult("Rol güncellendi.") : new ErrorResult("Ýþlem sýrasýnda bir hata oluþtu");
    }
}