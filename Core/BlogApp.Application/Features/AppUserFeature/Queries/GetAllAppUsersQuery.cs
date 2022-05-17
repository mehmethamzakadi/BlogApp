using BlogApp.Application.DTOs.Common;
using BlogApp.Application.DTOs.Results;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Application.Features.AppUserFeature.Queries
{
    public class GetAllAppUsersQuery : IRequest<BaseResult<RsAppUser>>
    {
        //public class GetAllAppUsersQueryHandler : IRequestHandler<GetAllAppUsersQuery, BaseResult<RsAppUser>>
        //{

        //    public Task<BaseResult<RsAppUser>> Handle(GetAllAppUsersQuery request, CancellationToken cancellationToken)
        //    {
                
        //    }
        //}
    }
}
