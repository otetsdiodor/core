using CoreBattle.Domain.Core.GameDomain;
using CoreBattle.Domain.Core.ManageDomain;
using CoreBattle.Domain.Interfaces;
using CoreBattle.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBattle.Controllers
{
    public class GameHub : Hub
    {
        private IMemoryCache _cache;
        private UserManager<User> _userManager;
        private IRepository<ResultStats> _statRepository;
        private IRepository<Game> _gameRepository;
        private IRepository<Cell> _cellRepository;
        private IRepository<Ship> _shipRepository;
        private IRepository<StepHistory> _stepRepository;
        private IRepository<GameBoard> _gBRepository;

        public GameHub(IMemoryCache cache,
            IRepository<ResultStats> statRepository,
            UserManager<User> userManager,
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
            _userManager = userManager;
        }

        public async override Task OnConnectedAsync()
        {
            _cache.TryGetValue(Context.UserIdentifier, out string gameId);
            _cache.TryGetValue(gameId, out Game game);
            var player = game.GameBoards.FirstOrDefault(p => p.Player.User.Id == Context.UserIdentifier).Player;
            var oponent = game.GameBoards.FirstOrDefault(b => b.Player.Id != player.Id)?.Player?.User;
            if (oponent != null)
            {
                await Clients.User(Context.UserIdentifier).SendAsync("MyName", player.User.NickName);
                await Clients.User(Context.UserIdentifier).SendAsync("OponentName", oponent.NickName);
                await Clients.User(oponent.Id).SendAsync("OponentName", player.User.NickName);
            }
            else
                await Clients.User(Context.UserIdentifier).SendAsync("MyName", player.User.NickName);
        }

        public async Task Send(string message)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            await Clients.All.SendAsync("ErrorHandler", message);
        }

        public async Task PlaceShip(int x1, int y1, int x2, int y2)
        {
            _cache.TryGetValue(Context.UserIdentifier, out string gameId);
            try
            {
                _cache.TryGetValue(gameId, out Game game);
                var userId = Context.UserIdentifier;
                var player = game.GameBoards.FirstOrDefault(p => p.Player.User.Id == userId).Player;

                try
                {
                    var result = game.GameBoards.FirstOrDefault(b => b.Player.Id == player.Id);
                    var ship = result.PlaceShip(new Coords(x1, y1), new Coords(x2, y2));
                    _shipRepository.Insert(ship);
                    foreach (var item in result.Field)
                    {
                        foreach (var cell in item.CellsRow)
                        {
                            _cellRepository.Update(cell);
                        }
                    }
                    var responseField = ConvertField(result.Field);
                    await Clients.User(userId).SendAsync("PlaceShipResult", responseField);
                }
                catch (Exception e)
                {
                    await Clients.User(userId).SendAsync("ErrorHandler", e.Message);
                }
                _cache.Remove(game.Id);
                _cache.Set(game.Id.ToString(), game);
            }
            catch (Exception ee)
            {
                Console.WriteLine("LOSER");
            }
        }
        private List<object> ConvertField(List<Row> field)
        {
            var result = new List<object>();
            foreach (var item in field)
            {
                foreach (var cell in item.CellsRow)
                {
                    result.Add(new { X = cell.X, Y = cell.Y, State = cell.State });
                }
            }
            return result;
        }
        public async Task Shoot(int x,int y)
        {
            _cache.TryGetValue(Context.UserIdentifier, out string gameId);
            _cache.TryGetValue(gameId, out Game game);
            var CurrId = Context.UserIdentifier;
            var player = game.GameBoards.FirstOrDefault(p => p.Player.User.Id == CurrId).Player;
            var userId = game.GameBoards.FirstOrDefault(b => b.Player.Id != player.Id).Player.User.Id;
            try
            {
                var ShootedCell = game.Shoot(player,new Coords(x,y));
                ShootedCell.Row = null;
                _cellRepository.Update(ShootedCell);
                _stepRepository.Insert(game.GameHistory.Last());
                var my = ConvertField(game.GameBoards.FirstOrDefault(b => b.Player.Id == player.Id).Field);
                var enemy = ConvertField(game.GameBoards.FirstOrDefault(b => b.Player.Id != player.Id).Field);
                await Clients.User(CurrId).SendAsync("ShootResult", my, enemy);
                await Clients.User(userId).SendAsync("ShootResult", enemy, my);
                if (game.IsGameEnded(player))
                {
                    var g = _gameRepository.Get(Guid.Parse(gameId));
                    g.Status = GameStatus.Completed;
                    _gameRepository.Update(g);
                    var stat = new ResultStats(player, game);
                    _statRepository.Insert(stat);
                    await Clients.User(CurrId).SendAsync("EndGame", "WIN");
                    await Clients.User(userId).SendAsync("EndGame", "LOSE");
                }
            }
            catch (Exception e)
            {
                await Clients.User(CurrId).SendAsync("ErrorHandler", e.Message);
            }
            _cache.Remove(game.Id);
            _cache.Set(game.Id.ToString(), game);
        }

        public async Task Ready()
        {
            _cache.TryGetValue(Context.UserIdentifier, out string gameId);
            _cache.TryGetValue(gameId, out Game game);
            var CurrId = Context.UserIdentifier;
            var player = game.GameBoards.FirstOrDefault(p => p.Player.User.Id == CurrId).Player;
            try
            {
                var board = game.GameBoards.FirstOrDefault(b => b.Player.Id == player.Id);
                board.IsReady = true;
                _gBRepository.Update(board);
                if (game.IsValidToStart())
                {
                    var userId = game.GameBoards.FirstOrDefault(b => b.Player.Id != player.Id).Player.User.Id;
                    var startId = game.Current.User.Id;
                    await Clients.User(startId).SendAsync("ErrorHandler", "YOUR_TURN");
                    await Clients.Users(CurrId, userId).SendAsync("START_GAME");
                }
            }
            catch (Exception e)
            {
                await Clients.User(CurrId).SendAsync("ErrorHandler", e.Message);
            }
            
            _cache.Remove(game.Id);
            _cache.Set(game.Id.ToString(), game);

            await Clients.User(CurrId).SendAsync("ReadyResult", "OK");
        }
    }
}
