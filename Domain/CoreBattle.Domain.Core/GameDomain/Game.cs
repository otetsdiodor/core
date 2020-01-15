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
        public int BoardLength { get; set; }
        public List<GameBoard> GameBoards { get; set; }
        public List<StepHistory> GameHistory { get; set; }
        public GameStatus Status { get; set; }
        private Player Current;

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

        public void Shoot(Player player, Coords c)
        {
            if (Current == player)
                throw new Exception("ITS NOT YOUR TURN ALLO, ARE YOU CRAZY???????");

            var result = GameBoards.Where(b => b.Player != player)
                .FirstOrDefault()
                .Shoot(c);

            Current = GameBoards.FirstOrDefault(b => b.Player != Current).Player;

            GameHistory.Add(new StepHistory(player, this, result));
            if (IsGameEnded(player))
            {
                Console.WriteLine("GAME IS ENDED");
            }

        }

        private bool IsGameEnded(Player p)
        {
            var ships = GameBoards.Where(b => b.Player != p)
                .FirstOrDefault()
                .Ships;

            foreach (var ship in ships)
                if (ship.State != ShipState.Destroyed)
                    return false;

            Status = GameStatus.Completed;
            return true;
        }

        public bool IsValidToStart(GameBoard board)
        {
            Dictionary<int, int> table = new Dictionary<int, int>
            {
                { 1, 0 },
                { 2, 0 },
                { 3, 0 },
                { 4, 0 }
            };
            foreach (var item in board.Ships)
                table[item.Length]++;

            if (table[1] == 4 && table[2] == 3 && table[3] == 2 && table[4] == 1)
                return true;

            return false;
        }
    }
}
