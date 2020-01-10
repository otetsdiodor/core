using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public enum BoardStatus
    {
        YourTurn,
        NotYourTurn
    }
    public class GameBoard : Entity
    {
        public int Length { get; set; }
        public List<List<Cell>> Field { get; set; }
        public List<Ship> Ships { get; set; }
        public Player Player { get; set; }
        public BoardStatus Status { get; set; }
        public GameBoard(Player player, int length)
        {
            Player = player;
            Length = length;
            Field = new List<List<Cell>>();
            Ships = new List<Ship>();
            InitializeField();
            //InitializeShips();
        }

        private void InitializeField()
        {
            for (int i = 0; i < Length; i++)
                for (int j = 0; j < Length; j++)
                    Field[i][j] = new Cell(i, j);
        }
        //private void InitializeShips()
        //{
        //    Ships.Add(new Ship(1));
        //    Ships.Add(new Ship(1));
        //    Ships.Add(new Ship(1));
        //    Ships.Add(new Ship(1));
        //    Ships.Add(new Ship(4));
        //    Ships.Add(new Ship(3));
        //    Ships.Add(new Ship(3));
        //    Ships.Add(new Ship(2));
        //    Ships.Add(new Ship(2));
        //    Ships.Add(new Ship(2));
        //}

        public void Shoot(int x, int y)
        {
            var cell = Field[x][y];
            if (cell.State == CellState.Hidden)
            {
                foreach (var ship in Ships)
                {
                    foreach (var coord in ship.Coords)
                    {
                        if (coord.X == x && coord.Y == y)
                        {
                            Console.WriteLine("DEAD");

                        }
                    }
                }
            }
        }

        public void PlaceShip(Guid shipId, int x, int y)
        {
            var cell = Field[x][y];

        }
    }
}
