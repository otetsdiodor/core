using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public enum Direction
    {
        Horizontal,
        Vertival
    }
    public class GameBoard : Entity
    {
        public int Length { get; set; }
        public bool IsReady { get; set; }
        public List<Row> Field { get; set; }
        public List<Ship> Ships { get; set; }
        public Guid PlayerId { get; set; }
        public Player Player { get; set; }
        public Game Game { get; set; }
        public Guid GameId { get; set; }
        public GameBoard(Player player, int length)
        {
            IsReady = false;
            Player = player;
            Length = length;
            Field = new List<Row>();
            Ships = new List<Ship>();
            InitializeField();
        }

        public GameBoard()
        { }

        private void InitializeField()
        {
            for (int i = 0; i < Length; i++)
            {
                Field.Add(new Row(Length));
                for (int j = 0; j < Length; j++)
                    Field[i].CellsRow.Add(new Cell(i, j));
            }
        }
    }
}
