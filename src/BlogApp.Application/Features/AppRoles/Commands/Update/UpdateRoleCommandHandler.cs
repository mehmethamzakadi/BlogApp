using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.Update;

public sealed class UpdateRoleCommandHandler(IRoleService roleService) : IRequestHandler<UpdateRoleCommand, IResult>
{
    public async Task<IResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var checkRole = roleService.AnyRole(request.Name);
        if (checkRole)
            return new ErrorResult($"G�ncellemek istedi�iniz {request.Name} rol� sistemde mevcut!");

        var result = await roleService.UpdateRole(new AppRole { Id = request.Id, Name = request.Name });

        return result.Succeeded ? new SuccessResult("Rol g�ncellendi.") : new ErrorResult("��lem s�ras�nda bir hata olu�tu");
    }
}