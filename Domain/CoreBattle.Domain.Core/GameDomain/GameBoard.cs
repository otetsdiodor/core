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
        public List<Row> Field { get; set; }
        public List<Ship> Ships { get; set; }
        public Player Player { get; set; }
        public bool IsReady { get; set; }
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

        public Cell Shoot(Coords c)
        {
            switch (Field[c.X].CellsRow[c.Y].State)
            {
                case CellState.Water:
                    {
                        Field[c.X].CellsRow[c.Y].State = CellState.DamagedWater;
                    }
                    break;
                case CellState.DamagedWater:
                    {
                        throw new Exception("Already shooted here earlier");
                    }
                    break;
                case CellState.Ship:
                    {
                        Field[c.X].CellsRow[c.Y].State = CellState.DamagedShip;
                        var ship = GetShip(c);

                        if (ship != null)
                            ship.State = ShipState.Damaged;
                        
                        var flag = true;
                        foreach (var part in ship.Coords)
                            if (Field[part.X].CellsRow[part.Y].State != CellState.DamagedShip)
                                flag = false;
                        if (flag)
                            ship.State = ShipState.Destroyed;
                    }
                    break;
                case CellState.DamagedShip:
                    {
                        throw new Exception("Already shooted here earlier");
                    }
                    break;
            }
            return Field[c.X].CellsRow[c.Y];
        }


        private Ship GetShip(Coords c)
        {
            foreach (var ship in Ships)
                foreach (var coord in ship.Coords)
                    if (coord.X == c.X && coord.Y == c.Y)
                        return ship;

            return null;
        }
        public List<Coords> PlaceShip(Coords c1, Coords c2)
        {
            if (!IsCoordsValid(c1, c2))
                throw new Exception("Coords not valid");

            int start ,end;
            var coords = new List<Coords>();
            var direction = GetDirection(c1, c2);

            switch (direction)
            {
                case Direction.Horizontal:
                    {
                        start = c1.Y; end = c2.Y;
                        if (!IsCanPlaceShip(c2.Y - c1.Y + 1))
                            throw new Exception("Cant place ship because count of ships greater than expected");

                        for (int i = c1.Y; i <= c2.Y; i++)
                            if (Field[c1.X].CellsRow[i].BlockedForPlacing == false)
                            {
                                coords.Add(new Coords(c1.X, i));
                                Field[c1.X].CellsRow[i].State = CellState.Ship;
                            }
                            else
                                throw new Exception("Trying to place ship on blocked cell");

                        for (int i = c1.X - 1; i <= c1.X + 1; i++)
                            for (int j = start - 1; j <= end + 1; j++)
                                if (!IsOutOfRange(i) && !IsOutOfRange(j))
                                    Field[i].CellsRow[j].BlockedForPlacing = true;
                    }
                    break;
                case Direction.Vertival:
                    {
                        start = c1.X; end = c2.X;
                        if (!IsCanPlaceShip(c2.X - c1.X + 1))
                            throw new Exception("Cant place ship because count of ships greater than expected");

                        for (int i = c1.X; i <= c2.X; i++)
                            if (Field[i].CellsRow[c1.Y].BlockedForPlacing == false)
                            {
                                coords.Add(new Coords(i, c1.Y));
                                Field[i].CellsRow[c1.Y].State = CellState.Ship;
                            }
                            else
                                throw new Exception("Trying to place ship on blocked cell");

                        for (int i = start - 1; i <= end + 1; i++)
                            for (int j = c2.Y - 1; j <= c2.Y + 1; j++)
                                if (!IsOutOfRange(i) && !IsOutOfRange(j))
                                    Field[i].CellsRow[j].BlockedForPlacing = true;
                    }
                    break;
            }
            Ships.Add(new Ship(coords.Count, coords));
            return coords;
        }

        private bool IsCanPlaceShip(int len)
        {
            Dictionary<int, int> table = new Dictionary<int, int>
            {
                { 1, 0 },
                { 2, 0 },
                { 3, 0 },
                { 4, 0 }
            };
            foreach (var item in Ships)
                table[item.Length]++;

            switch (len)
            {
                case 1: if (table[len] < 4) return true; break;
                case 2: if (table[len] < 3) return true; break;
                case 3: if (table[len] < 2) return true; break;
                case 4: if (table[len] < 1) return true; break;
            }
            return false;
        }

        private bool IsOutOfRange(int x)
        {
            if (x > Length - 1 || x < 0)
            {
                return true;
            }
            return false;
        }

        private Direction GetDirection(Coords c1, Coords c2)
        {
            if (c1.X == c2.X)
                return Direction.Horizontal;
            else
                return Direction.Vertival;
        }

        private bool IsCoordsValid(Coords c1, Coords c2)
        {
            if (c1.X == c2.X || c1.Y == c2.Y)
            {
                return true;
            }
            return false;
        }
    }
}
