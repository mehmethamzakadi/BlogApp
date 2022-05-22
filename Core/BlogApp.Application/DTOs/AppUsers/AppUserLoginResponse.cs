using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp.Application.DTOs.AppUsers
{
    public class AppUserLoginResponse
    {
        public DateTime Expiration { get; set; }
        public string? Token { get; set; }
    }
}
