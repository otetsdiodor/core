using CoreBattle.Domain.Core.GameDomain;
using CoreBattle.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreBattle.Services.Interfaces
{
    public class GameManager
    {
        public Game game;
        GameBoard My;
        GameBoard Op;
        Player current;


        private IMemoryCache _cache;
        private IRepository<ResultStats> _statRepository;
        private IRepository<Game> _gameRepository;
        private IRepository<Cell> _cellRepository;
        private IRepository<Ship> _shipRepository;
        private IRepository<StepHistory> _stepRepository;
        private IRepository<GameBoard> _gBRepository;

        public GameManager()
        {

        }
        public GameManager(IMemoryCache cache,
            IRepository<ResultStats> statRepository,
            IRepository<Game> gameRepository,
            IRepository<GameBoard> gBRepository,
            IRepository<Cell> cellRepository,
            IRepository<StepHistory> stepRepository,
            IRepository<Ship> shipRepository)
        {
            _statRepository = statRepository;
            _stepRepository = stepRepository;
            _shipRepository = shipRepository;
            _cellRepository = cellRepository;
            _gameRepository = gameRepository;
            _gBRepository = gBRepository;
            _cache = cache;
        }
        public Cell RegistrateShoot(Coords c) {
            if (game.Current.Id != current.Id)
                throw new Exception("ITS NOT YOUR TURN ALLO, ARE YOU CRAZY???????");

            switch (My.Field[c.X].CellsRow[c.Y].State)
            {
                case CellState.Water:
                    {
                        My.Field[c.X].CellsRow[c.Y].State = CellState.DamagedWater;
                    }
                    break;
                case CellState.DamagedWater:
                    {
                        throw new Exception("Already shooted here earlier");
                    }
                case CellState.Ship:
                    {
                        My.Field[c.X].CellsRow[c.Y].State = CellState.DamagedShip;
                        var ship = GetShip(c);

                        if (ship != null)
                            ship.State = ShipState.Damaged;

                        var flag = true;
                        foreach (var part in ship.Coords)
                            if (My.Field[part.X].CellsRow[part.Y].State != CellState.DamagedShip)
                                flag = false;
                        if (flag)
                            ship.State = ShipState.Destroyed;
                    }
                    break;
                case CellState.DamagedShip:
                    {
                        throw new Exception("Already shooted here earlier");
                    }
            }

            if (My.Field[c.X].CellsRow[c.Y].State != CellState.DamagedShip)
                game.Current = game.GameBoards.FirstOrDefault(b => b.Player.Id != current.Id).Player;

            game.GameHistory.Add(new StepHistory(current, game, My.Field[c.X].CellsRow[c.Y]));

            return My.Field[c.X].CellsRow[c.Y];
        }

        public List<object> PlaceShip(Coords c1, Coords c2)
        {
            if (!IsCoordsValid(c1, c2))
                throw new Exception("Coords not valid");

            int start, end;
            var coords = new List<Coords>();
            var direction = GetDirection(c1, c2);
            ChangeCoordsPriority(ref c1,ref c2);
            switch (direction)
            {
                case Direction.Horizontal:
                    {
                        start = c1.Y; end = c2.Y;
                        if (!IsCanPlaceShip(c2.Y - c1.Y + 1))
                            throw new Exception("Cant place ship because count of ships greater than expected");

                        for (int i = c1.Y; i <= c2.Y; i++)
                            if (My.Field[c1.X].CellsRow[i].BlockedForPlacing == false)
                            {
                                coords.Add(new Coords(c1.X, i));
                                My.Field[c1.X].CellsRow[i].State = CellState.Ship;
                            }
                            else
                                throw new Exception("Trying to place ship on blocked cell");

                        for (int i = c1.X - 1; i <= c1.X + 1; i++)
                            for (int j = start - 1; j <= end + 1; j++)
                                if (!IsOutOfRange(i) && !IsOutOfRange(j))
                                    My.Field[i].CellsRow[j].BlockedForPlacing = true;
                    }
                    break;
                case Direction.Vertival:
                    {
                        start = c1.X; end = c2.X;
                        if (!IsCanPlaceShip(c2.X - c1.X + 1))
                            throw new Exception("Cant place ship because count of ships greater than expected");

                        for (int i = c1.X; i <= c2.X; i++)
                            if (My.Field[i].CellsRow[c1.Y].BlockedForPlacing == false)
                            {
                                coords.Add(new Coords(i, c1.Y));
                                My.Field[i].CellsRow[c1.Y].State = CellState.Ship;
                            }
                            else
                                throw new Exception("Trying to place ship on blocked cell");

                        for (int i = start - 1; i <= end + 1; i++)
                            for (int j = c2.Y - 1; j <= c2.Y + 1; j++)
                                if (!IsOutOfRange(i) && !IsOutOfRange(j))
                                    My.Field[i].CellsRow[j].BlockedForPlacing = true;
                    }
                    break;
            }
            My.Ships.Add(new Ship(coords.Count, coords, My));

            _shipRepository.Insert(My.Ships.LastOrDefault());

            foreach (var item in My.Field)
                foreach (var cell in item.CellsRow)
                    _cellRepository.Update(cell);

            _cache.Set(game.Id.ToString(), game);

            return ConvertFieldToSend(My.Field);
        }
        public bool IsGameEnded(Player p)
        {
            var ships = game.GameBoards.Where(b => b.Player.Id != p.Id)
                .FirstOrDefault()
                .Ships;

            foreach (var ship in ships)
                if (ship.State != ShipState.Destroyed)
                    return false;

            game.Status = GameStatus.Completed;
            return true;
        }

        public bool IsValidToStart()
        {
            if (game.GameBoards.Count == 2 && game.GameBoards.All(b => b.IsReady))
            {
                game.Current = game.GameBoards[new Random().Next(0, 2)].Player;
                return true;
            }

            return false;
        }
        public void PlayerReady() { }

        #region PRIVATE LOGIC
        private void ChangeCoordsPriority(ref Coords c1, ref Coords c2)
        {
            if (c1.X > c2.X || c1.Y > c2.Y)
            {
                var tmp = c1;
                c1 = c2;
                c2 = tmp;
            }
        }
        private List<object> ConvertFieldToSend(List<Row> field)
        {
            var result = new List<object>();
            foreach (var item in field)
                foreach (var cell in item.CellsRow)
                    result.Add(new { cell.X, cell.Y, cell.State });

            return result;
        }

        private bool IsCoordsValid(Coords c1, Coords c2)
        {
            if (c1.X == c2.X || c1.Y == c2.Y)
            {
                return true;
            }
            return false;
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
            foreach (var item in My.Ships)
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
            if (x > My.Length - 1 || x < 0)
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
        private Ship GetShip(Coords c)
        {
            foreach (var ship in My.Ships)
                foreach (var coord in ship.Coords)
                    if (coord.X == c.X && coord.Y == c.Y)
                        return ship;

            return null;
        }
        #endregion
    }
}
