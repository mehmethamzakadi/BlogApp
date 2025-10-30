using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;
using IResult = BlogApp.Domain.Common.Results.IResult;

namespace BlogApp.Application.Features.Roles.Commands.Delete;

public sealed class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, IResult>
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoleCommandHandler(
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = _roleRepository.GetRoleById(request.Id);
        if (role == null)
            return new ErrorResult("Rol bulunamadı!");

        if (role.NormalizedName == "ADMIN")
            return new ErrorResult("Admin rolü silinemez!");

        role.Delete();
        var result = await _roleRepository.DeleteRole(role);
        if (!result.Success)
            return new ErrorResult("Rol silme sırasında hata oluştu!");

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Rol silindi.");
    }
}
