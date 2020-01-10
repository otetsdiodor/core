using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public class Game : Entity
    {
        private const int maxPlayerCount = 2;
        private int BoardLength; // ?????
        public List<GameBoard> GameBoards { get; set; }
        public List<StepHistory> GameHistory { get; set; }
        public bool IsEnded { get; set; }

        public Game(int boardLength)
        {
            BoardLength = boardLength;
            GameBoards = new List<GameBoard>(maxPlayerCount);
            GameHistory = new List<StepHistory>();
        }

        public void AddBoard(Player player)
        {
            if (GameBoards.Count < maxPlayerCount)
                GameBoards.Add(new GameBoard(player, BoardLength));
        }

        //public void PlaceShip(Guid playerId,Guid shipId, int x, int y)
        //{
        //    var player = Players.FirstOrDefault(p => p.Id == playerId);
        //    if (player == null)
        //        throw new Exception("PLayer is null");

        //    player.GameBoard.PlaceShip();


        //}

        //public void Shoot(Player player,int x,int y)
        //{
        //    GameBoards.Where(b=> b.Player == player)
        //        .FirstOrDefault()
        //        .Shoot(x, y);
        //}
    }
}
