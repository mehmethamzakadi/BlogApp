using AutoMapper;
using BlogApp.Application.Abstractions;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Queries.GetById;

public sealed class GetByIdUserQueryHandler(
    IUserService userManager,
    IMapper mapper) : IRequestHandler<GetByIdAppUserQuery, Result<GetByIdAppUserResponse>>
{
    public async Task<Result<GetByIdAppUserResponse>> Handle(GetByIdAppUserQuery request, CancellationToken cancellationToken)
    {
        AppUser? user = userManager.FindById(request.Id);
        if (user is null)
            return Result<GetByIdAppUserResponse>.FailureResult("Kullanıcı bulunamadı!");

        var userResponse = mapper.Map<GetByIdAppUserResponse>(user);
        return Result<GetByIdAppUserResponse>.SuccessResult(userResponse);
    }
}
