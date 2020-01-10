using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public class StepHistory : Entity
    {
        public Guid PlayerId { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public CellState CellState { get; set; }
        public ShipState ShipState { get; set; }
        public Guid GameId { get; set; }
        public StepHistory(Player pl, Game g, Cell cell, ShipState shipState)
        {
            PlayerId = pl.Id;
            x = cell.X;
            y = cell.Y;
            CellState = cell.State;
            ShipState = shipState;
            GameId = g.Id;
        }
    }
}
