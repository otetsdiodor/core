using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public class ResultStats : Entity
    {
        public Player Winner { get; set; }
        public int CountOfSteps { get; set; }
        public List<Ship> Ships { get; set; }
    }
}
