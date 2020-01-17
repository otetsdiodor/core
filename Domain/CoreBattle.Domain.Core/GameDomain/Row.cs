using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public class Row : Entity
    {
        public List<Cell> CellsRow { get; set; }
        public GameBoard GameBoard { get; set; }
        public Row(int length)
        {
            CellsRow = new List<Cell>(length);
        }
        public Row()
        {

        }
    }
}
