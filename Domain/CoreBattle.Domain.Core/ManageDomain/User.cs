using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBattle.Domain.Core.ManageDomain
{
    //public class User 
    //{
    //    public Guid Id { get; set; }
    //    public string UserName { get; set; }
    //    public string PasswordHash { get; set; }
    //}
    public class User : IdentityUser
    {
        public string NickName { get; set; }
    }
}
