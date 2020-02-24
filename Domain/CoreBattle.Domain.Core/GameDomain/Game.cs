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
        public Player Current { get; set; }

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


    }
}
