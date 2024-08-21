using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.Create;
public class CreateRoleCommandHandler(IRoleService roleService) : IRequestHandler<CreateRoleCommand, IResult>
{
    public async Task<IResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var checkRole = roleService.AnyRole(request.Name);
        if (checkRole)
            return new ErrorResult("Eklemek istediðiniz Rol sistemde mevcut!");

        var result = await roleService.CreateRole(new AppRole { Name = request.Name });
        return result.Succeeded ? new SuccessResult("Rol oluþturuldu.") : new ErrorResult("Ýþlem sýrasýnda hata oluþtu!");
    }
}