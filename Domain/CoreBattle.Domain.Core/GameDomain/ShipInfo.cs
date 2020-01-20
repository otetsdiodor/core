using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public class ShipInfo : Entity
    {
        public int Length { get; set; }
        public int CountLivePart { get; set; }

        public ShipInfo()
        {

        }
        public ShipInfo(int len, int count)
        {
            Length = len;
            CountLivePart = count;
        }
    }
}
