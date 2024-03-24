using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppRoles.Commands.Create;
public class CreateRoleCommandHandler(IRoleService roleService) : IRequestHandler<CreateRoleCommand, IResult>
{
    public async Task<IResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var result = await roleService.CreateRole(new AppRole { Name = request.Name });
        return result ? new SuccessResult("Rol olu�turuldu.") : new ErrorResult("Rol ekleme s�ras�nda hata olu�tu!");
    }
}