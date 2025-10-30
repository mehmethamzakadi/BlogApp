using BlogApp.Domain.Common;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        var role = await _roleRepository.GetAsync(
            predicate: r => r.Id == request.Id,
            include: r => r.Include(x => x.UserRoles),
            cancellationToken: cancellationToken);

        if (role == null)
            return new ErrorResult("Rol bulunamadı!");

        if (role.NormalizedName == "ADMIN")
            return new ErrorResult("Admin rolü silinemez!");

        if (role.UserRoles.Any(ur => !ur.IsDeleted))
            return new ErrorResult("Bu role atanmış aktif kullanıcılar bulunmaktadır. Önce kullanıcılardan bu rolü kaldırmalısınız.");

        role.Delete();
        await _roleRepository.DeleteAsync(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SuccessResult("Rol silindi.");
    }
}
