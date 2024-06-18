using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.Create;
public class CreateRoleCommandHandler(IRoleService roleService) : IRequestHandler<CreateRoleCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var checkRole = roleService.AnyRole(request.Name);
        if (checkRole)
            return Result<string>.FailureResult("Eklemek istediðiniz Rol sistemde mevcut!");

        var result = await roleService.CreateRole(new AppRole { Name = request.Name });

        return result.Succeeded
            ? Result<string>.SuccessResult("Rol oluþturuldu.")
            : Result<string>.FailureResult("Ýþlem sýrasýnda hata oluþtu!");
    }
}