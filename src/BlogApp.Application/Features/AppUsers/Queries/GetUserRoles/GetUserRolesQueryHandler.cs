using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Common.Results;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Application.Features.AppUsers.Queries.GetUserRoles;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, IDataResult<GetUserRolesResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public GetUserRolesQueryHandler(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IDataResult<GetUserRolesResponse>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            return new ErrorDataResult<GetUserRolesResponse>("Kullanıcı bulunamadı");
        }

        var roleNames = await _userManager.GetRolesAsync(user);
        var userRoles = new List<UserRoleDto>();
        
        foreach (var roleName in roleNames)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                userRoles.Add(new UserRoleDto 
                { 
                    Id = role.Id, 
                    Name = role.Name ?? string.Empty 
                });
            }
        }

        var response = new GetUserRolesResponse
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = userRoles
        };

        return new SuccessDataResult<GetUserRolesResponse>(response, "Kullanıcı rolleri başarıyla getirildi");
    }
}
