using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.Delete;

public class DeleteRoleCommandHandler(IRoleService roleService) : IRequestHandler<DeleteRoleCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await roleService.DeleteRole(new AppRole { Id = request.Id });
        return result.Succeeded
            ? Result<string>.SuccessResult("Rol silindi.")
            : Result<string>.FailureResult("Rol silme sýrasýnda hata oluþtu!");
    }
}