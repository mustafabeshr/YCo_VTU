using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yvtu.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
    }
}
