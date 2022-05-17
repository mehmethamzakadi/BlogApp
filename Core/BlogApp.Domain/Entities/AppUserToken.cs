using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogApp.Domain.Entities
{
    public class AppUserToken : IdentityUserToken<int>
    {
    }
}
