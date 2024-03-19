using AutoMapper;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Queries.GetById;

public sealed class GetByIdUserQueryHandler(
    UserManager<AppUser> userManager,
    IMapper mapper) : IRequestHandler<GetByIdAppUserQuery, IDataResult<GetByIdAppUserResponse>>
{
    public async Task<IDataResult<GetByIdAppUserResponse>> Handle(GetByIdAppUserQuery request, CancellationToken cancellationToken)
    {
        AppUser? user = userManager.Users.Where(x => x.Id == request.Id).FirstOrDefault();
        if (user is null)
            return new ErrorDataResult<GetByIdAppUserResponse>("Kullanıcı bulunamadı!");

        var userResponse = mapper.Map<GetByIdAppUserResponse>(user);
        return new SuccessDataResult<GetByIdAppUserResponse>(userResponse);
    }
}
