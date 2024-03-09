using AutoMapper;
using BlogApp.Application.Utilities.Results;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Queries.GetById
{
    public class GetByIdAppUserQuery : IRequest<IDataResult<GetByIdAppUserResponse>>
    {
        public int Id { get; set; }

        public class GetByIdUserQueryHandler : IRequestHandler<GetByIdAppUserQuery, IDataResult<GetByIdAppUserResponse>>
        {
            private readonly UserManager<AppUser> _userManager;

            private readonly IMapper _mapper;

            public GetByIdUserQueryHandler(UserManager<AppUser> userManager, IMapper mapper, SignInManager<AppUser> signInManager)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<IDataResult<GetByIdAppUserResponse>> Handle(GetByIdAppUserQuery request, CancellationToken cancellationToken)
            {
                AppUser? user = _userManager.Users.Where(x => x.Id == request.Id).FirstOrDefault();
                if (user is null)
                    return new ErrorDataResult<GetByIdAppUserResponse>("Kullanıcı bulunamadı!");

                var userDto = _mapper.Map<GetByIdAppUserResponse>(user);
                return new SuccessDataResult<GetByIdAppUserResponse>(userDto);
            }
        }
    }
}
