using Microsoft.AspNetCore.Identity;

namespace BlogApp.Domain.Entities;

public sealed class AppUserToken : IdentityUserToken<int>
{
}
