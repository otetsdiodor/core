using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public class StepHistory : Entity
    {
        public Player Player { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public CellState CellState { get; set; }
        public Game Game { get; set; }

        public StepHistory(Player pl, Game g, Cell cell)
        {
            Player = pl;
            X = cell.X;
            Y = cell.Y;
            CellState = cell.State;
            Game = g;
        }

        public StepHistory()
        {}
    }
}
