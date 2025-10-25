using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.Delete;

public sealed class DeleteRoleCommandHandler(IRoleService roleService) : IRequestHandler<DeleteRoleCommand, IResult>
{
    public async Task<IResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await roleService.DeleteRole(new AppRole { Id = request.Id });
        return result.Succeeded
            ? new SuccessResult("Rol silindi.")
            : new ErrorResult("Rol silme s�ras�nda hata olu�tu!");
    }
}