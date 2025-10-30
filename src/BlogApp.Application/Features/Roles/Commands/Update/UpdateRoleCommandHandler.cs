using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Roles.Commands.Update;

public sealed class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, IResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleCommandHandler(
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = _roleRepository.GetRoleById(request.Id);
        if (role == null)
            return new ErrorResult("Rol bulunamadı!");

        var existingRole = await _roleRepository.FindByNameAsync(request.Name);
        if (existingRole != null && existingRole.Id != request.Id)
            return new ErrorResult($"Güncellemek istediğiniz {request.Name} rolü sistemde mevcut!");

        role.Update(request.Name);
        role.ConcurrencyStamp = Guid.NewGuid().ToString();
        var result = await _roleRepository.UpdateRole(role);
        if (!result.Success)
            return new ErrorResult("İşlem sırasında bir hata oluştu");

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Rol güncellendi.");
    }
}
