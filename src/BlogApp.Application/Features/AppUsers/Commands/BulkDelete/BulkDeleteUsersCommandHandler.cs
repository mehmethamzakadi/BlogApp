using BlogApp.Application.Abstractions.Identity;
using BlogApp.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Application.Features.AppUsers.Commands.BulkDelete;

public class BulkDeleteUsersCommandHandler : IRequestHandler<BulkDeleteUsersCommand, BulkDeleteUsersResponse>
{
    private readonly IUserService _userService;
    private readonly UserManager<AppUser> _userManager;

    public BulkDeleteUsersCommandHandler(
        IUserService userService,
        UserManager<AppUser> userManager)
    {
        _userService = userService;
        _userManager = userManager;
    }

    public async Task<BulkDeleteUsersResponse> Handle(BulkDeleteUsersCommand request, CancellationToken cancellationToken)
    {
        var response = new BulkDeleteUsersResponse();

        foreach (var userId in request.UserIds)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                
                if (user == null)
                {
                    response.Errors.Add($"Kullanıcı bulunamadı: ID {userId}");
                    response.FailedCount++;
                    continue;
                }

                var result = await _userManager.DeleteAsync(user);
                
                if (result.Succeeded)
                {
                    response.DeletedCount++;
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    response.Errors.Add($"Kullanıcı silinemedi (ID {userId}): {errors}");
                    response.FailedCount++;
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add($"Kullanıcı silinirken hata oluştu (ID {userId}): {ex.Message}");
                response.FailedCount++;
            }
        }

        return response;
    }
}
