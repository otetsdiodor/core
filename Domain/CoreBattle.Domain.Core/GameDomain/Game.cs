using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public enum GameStatus
    {
        Waiting,
        GoesOn,
        Completed,
    }

    public class Game : Entity
    {
        private const int maxPlayerCount = 2;
        public Player Current { get; private set; }

        public int BoardLength { get; set; }
        public List<GameBoard> GameBoards { get; set; }
        public List<StepHistory> GameHistory { get; set; }
        public GameStatus Status { get; set; }

        public Game()
        { }

        public Game(int boardLength, Player player)
        {
            Status = GameStatus.Waiting;
            BoardLength = boardLength;
            GameBoards = new List<GameBoard>(maxPlayerCount);
            GameHistory = new List<StepHistory>();
            AddBoard(player);
            Current = player;
        }

        public void AddBoard(Player player)
        {
            if (GameBoards.Count < maxPlayerCount)
                GameBoards.Add(new GameBoard(player, BoardLength));
            else
                throw new ArgumentOutOfRangeException();
        }

        public Cell Shoot(Player player, Coords c)
        {
            if (Current.Id != player.Id)
                throw new Exception("ITS NOT YOUR TURN ALLO, ARE YOU CRAZY???????");

            var result = GameBoards.Where(b => b.Player.Id != player.Id)
                .FirstOrDefault()
                .Shoot(c);
            if (result.State != CellState.DamagedShip)
                Current = GameBoards.FirstOrDefault(b => b.Player.Id != Current.Id).Player;

            GameHistory.Add(new StepHistory(player, this, result));
            return result;
        }

        public bool IsGameEnded(Player p)
        {
            var ships = GameBoards.Where(b => b.Player.Id != p.Id)
                .FirstOrDefault()
                .Ships;

            foreach (var ship in ships)
                if (ship.State != ShipState.Destroyed)
                    return false;

            Status = GameStatus.Completed;
            return true;
        }

        public bool IsValidToStart()
        {
            if (GameBoards.Count == 2 && GameBoards.All(b => b.IsReady))
            {
                Current = GameBoards[new Random().Next(0, 2)].Player;
                return true;
            }

            return false;
        }
    }
}
