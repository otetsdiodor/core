using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CoreBattle.Domain.Core.ManageDomain
{
    public class User : IdentityUser
    {
        public string NickName { get; set; }
    }
}
