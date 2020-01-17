using CoreBattle.Domain.Core.ManageDomain;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public class Player : Entity
    {
        public User User { get; set; }

        public Player(User user)
        {
            User = user;
        }

        public Player()
        {}
    }
}
