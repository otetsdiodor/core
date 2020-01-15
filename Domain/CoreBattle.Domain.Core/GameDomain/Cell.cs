using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public enum CellState
    {
        Water,
        DamagedWater,
        Ship,
        DamagedShip
    }
    public enum CellDSC { 
        BlockedForPlacing
    }

    public class Cell : Entity
    {
        public int X { get; set; }
        public int Y { get; set; }
        public CellState State { get; set; }
        public bool BlockedForPlacing { get; set; }
        public Cell(int x, int y)
        {
            X = x;
            Y = y;
            State = CellState.Water;
        }
        public Cell()
        {

        }
    }
}
