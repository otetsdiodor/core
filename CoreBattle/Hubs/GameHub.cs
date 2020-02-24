using CoreBattle.Domain.Core.GameDomain;
using CoreBattle.Domain.Core.ManageDomain;
using CoreBattle.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using CoreBattle.Services.Interfaces;

namespace CoreBattle.Hubs
{
    public class GameHub : Hub
    {
        private Player CurrentPlayer;
        private Player OponentPlayer;
        private GameManager Manager;

        public GameHub(GameManager gameManager)
        {
            Manager = gameManager;
        }

        public async override Task OnConnectedAsync()
        {
            //InitializeHub();
            if (OponentPlayer != null)
            {
                await Clients.User(Context.UserIdentifier).SendAsync("MyName", CurrentPlayer.User.NickName);
                await Clients.User(Context.UserIdentifier).SendAsync("OponentName", OponentPlayer.User.NickName);
                await Clients.User(OponentPlayer.User.Id).SendAsync("OponentName", CurrentPlayer.User.NickName);
            }
            else
                await Clients.User(Context.UserIdentifier).SendAsync("MyName", CurrentPlayer.User.NickName);
        }

        private void InitializeHub()
        {
            CurrentPlayer = Manager.game.GameBoards.FirstOrDefault(p => p.Player.User.Id == Context.UserIdentifier).Player;
            OponentPlayer = Manager.game.GameBoards.FirstOrDefault(b => b.Player.Id != CurrentPlayer.Id)?.Player;
        }
        public async Task PlaceShip(int x1, int y1, int x2, int y2)
        {
            InitializeHub();
            try
            {
                var field = Manager.PlaceShip(new Coords(x1, y1), new Coords(x2, y2));
                await Clients.User(Context.UserIdentifier).SendAsync("PlaceShipResult", field);
            }
            catch (Exception e)
            {
                await Clients.User(Context.UserIdentifier).SendAsync("Alerter", e.Message);
            }
        }

        //public async Task Shoot(int x, int y)
        //{
        //    InitializeHub();
        //    try
        //    {
        //        var ShootedCell = game.Shoot(CurrentPlayer, new Coords(x, y));
        //        ShootedCell.Row = null;
        //        _cellRepository.Update(ShootedCell);
        //        _stepRepository.Insert(game.GameHistory.Last());
        //        var myField = ConvertFieldToSend(game.GameBoards.FirstOrDefault(b => b.Player.Id == CurrentPlayer.Id).Field);
        //        var enField = ConvertFieldToSend(game.GameBoards.FirstOrDefault(b => b.Player.Id != CurrentPlayer.Id).Field);
        //        await Clients.User(CurrentPlayer.User.Id).SendAsync("ShootResult", myField, enField);
        //        await Clients.User(OponentPlayer.User.Id).SendAsync("ShootResult", enField, myField);
        //        if (game.IsGameEnded(CurrentPlayer))
        //        {
        //            var g = _gameRepository.Get(game.Id);
        //            g.Status = GameStatus.Completed;
        //            _gameRepository.Update(g);
        //            _statRepository.Insert(new ResultStats(CurrentPlayer, game));
        //            await Clients.User(CurrentPlayer.User.Id).SendAsync("EndGame", "WIN");
        //            await Clients.User(OponentPlayer.User.Id).SendAsync("EndGame", "LOSE");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        await Clients.User(CurrentPlayer.User.Id).SendAsync("Alerter", e.Message);
        //    }
        //    _cache.Remove(game.Id);
        //    _cache.Set(game.Id.ToString(), game);
        //}

        //public async Task Ready()
        //{
        //    InitializeHub();
        //    try
        //    {
        //        var board = game.GameBoards.FirstOrDefault(b => b.Player.Id == CurrentPlayer.Id);
        //        board.IsReady = true;
        //        _gBRepository.Update(board);
        //        if (game.IsValidToStart())
        //        {
        //            var startId = game.Current.User.Id;
        //            await Clients.User(startId).SendAsync("Alerter", "YOUR_TURN");
        //            await Clients.Users(CurrentPlayer.User.Id, OponentPlayer.User.Id).SendAsync("START_GAME");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        await Clients.User(CurrentPlayer.User.Id).SendAsync("Alerter", e.Message);
        //    }

        //    _cache.Remove(game.Id);
        //    _cache.Set(game.Id.ToString(), game);

        //    await Clients.User(CurrentPlayer.User.Id).SendAsync("ReadyResult", "OK");
        //}
    }
}
