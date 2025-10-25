using AutoMapper;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Repositories;
using MediatR;

namespace BlogApp.Application.Features.Users.Queries.GetById;

public sealed class GetByIdUserQueryHandler(
    IUserRepository userRepository,
    IMapper mapper) : IRequestHandler<GetByIdUserQuery, IDataResult<GetByIdUserResponse>>
{
    public Task<IDataResult<GetByIdUserResponse>> Handle(GetByIdUserQuery request, CancellationToken cancellationToken)
    {
        User? user = userRepository.FindById(request.Id);
        if (user is null)
        {
            IDataResult<GetByIdUserResponse> errorResult = new ErrorDataResult<GetByIdUserResponse>("Kullanıcı bulunamadı!");
            return Task.FromResult(errorResult);
        }

        GetByIdUserResponse userResponse = mapper.Map<GetByIdUserResponse>(user);
        IDataResult<GetByIdUserResponse> successResult = new SuccessDataResult<GetByIdUserResponse>(userResponse);
        return Task.FromResult(successResult);
    }
}
