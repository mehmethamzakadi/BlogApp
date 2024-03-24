using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.Delete;

public class DeleteRoleCommandHandler(IRoleService roleService) : IRequestHandler<DeleteRoleCommand, IResult>
{
    public async Task<IResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        return await roleService.DeleteRole(new AppRole { Id = request.Id })
            ? new SuccessResult("Rol silindi.")
            : new ErrorResult("Rol silme sýrasýnda hata oluþtu!");
    }
}