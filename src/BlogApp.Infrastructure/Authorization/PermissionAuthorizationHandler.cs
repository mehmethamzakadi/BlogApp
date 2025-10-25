using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogApp.Infrastructure.Authorization;

/// <summary>
/// Permission requirement'larını handle eden authorization handler
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Kullanıcının permission claim'lerini kontrol et
        var permissionClaim = context.User.FindFirst(c => 
            c.Type == "permission" && c.Value == requirement.Permission);

        if (permissionClaim != null)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
