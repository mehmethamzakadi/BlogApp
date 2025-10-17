using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;

namespace BlogApp.Application.Features.AppUsers.Queries.GetById;

public sealed class GetByIdUserQueryHandler(
    IUserService userManager,
    IMapper mapper) : IRequestHandler<GetByIdAppUserQuery, IDataResult<GetByIdAppUserResponse>>
{
    public Task<IDataResult<GetByIdAppUserResponse>> Handle(GetByIdAppUserQuery request, CancellationToken cancellationToken)
    {
        AppUser? user = userManager.FindById(request.Id);
        if (user is null)
        {
            IDataResult<GetByIdAppUserResponse> errorResult = new ErrorDataResult<GetByIdAppUserResponse>("Kullanıcı bulunamadı!");
            return Task.FromResult(errorResult);
        }

        GetByIdAppUserResponse userResponse = mapper.Map<GetByIdAppUserResponse>(user);
        IDataResult<GetByIdAppUserResponse> successResult = new SuccessDataResult<GetByIdAppUserResponse>(userResponse);
        return Task.FromResult(successResult);
    }
}
