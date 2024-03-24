using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.Update;

public class UpdateRoleCommandHandler(IRoleService roleService) : IRequestHandler<UpdateRoleCommand, IResult>
{
    public async Task<IResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        return await roleService.UpdateRole(new AppRole { Id = request.Id, Name = request.Name })
                    ? new SuccessResult("Rol güncellendi.")
                    : new ErrorResult("Rol güncelleme sýrasýnda hata oluþtu!");
    }
}