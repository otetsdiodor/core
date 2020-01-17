using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public class Coords : Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Ship Ship { get; set; }
        public Coords()
        {}
        public Coords(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
