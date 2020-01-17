using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public enum ShipState
    {
        Damaged,
        Full,
        Destroyed
    }

    public class Ship : Entity
    {
        public List<Coords> Coords { get; set; }
        public ShipState State { get; set; }
        public int Length { get; set; }
        public GameBoard GameBoard { get; set; }
        public Ship(int length,List<Coords> coords)
        {
            Coords = coords;
            Length = length;
            State = ShipState.Full;
        }
        public Ship()
        {

        }
    }
}
