using CoreBattle.Domain.Core.GameDomain;
using CoreBattle.Domain.Core.ManageDomain;
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
        private Repository<Player> _playerRepository;
        private Repository<ResultStats> _statRepository;
        private Repository<Game> _gameRepository;
        private Repository<Cell> _cellRepository;
        private Repository<Ship> _shipRepository;
        private Repository<StepHistory> _stepRepository;
        Repository<GameBoard> _gBRepository;

        public GameHub(IMemoryCache cache,
            Repository<ResultStats> statRepository,
            UserManager<User> userManager,
            Repository<Player> playerRepository,
            Repository<Game> gameRepository,
            Repository<GameBoard> gBRepository,
            Repository<Cell> cellRepository,
            Repository<StepHistory> stepRepository,
            Repository<Ship> shipRepository)
        {
            _statRepository = statRepository;
            _stepRepository = stepRepository;
            _shipRepository = shipRepository;
            _cellRepository = cellRepository;
            _gameRepository = gameRepository;
            _gBRepository = gBRepository;
            _cache = cache;
            _userManager = userManager;
            _playerRepository = playerRepository;
        }

        public async Task Send(string message)
        {
            var user = await _userManager.GetUserAsync(Context.User);
            await Clients.All.SendAsync("Send", message);
            await Clients.User(user.Id).SendAsync("Send", "Placed");
        }

        public async Task PlaceShip(string gameId, string x1, string y1, string x2, string y2)
        {
            try
            {
                _cache.TryGetValue(gameId, out Game game);
                var userId = Context.UserIdentifier;
                var player = game.GameBoards.FirstOrDefault(p => p.Player.User.Id == userId).Player;

                try
                {
                    var result = game.GameBoards.FirstOrDefault(b => b.Player.Id == player.Id);
                    var ship = result.PlaceShip(new Coords(int.Parse(x1), int.Parse(y1)), new Coords(int.Parse(x2), int.Parse(y2)));
                    _shipRepository.Insert(ship);
                    foreach (var item in result.Field)
                    {
                        foreach (var cell in item.CellsRow)
                        {
                            _cellRepository.Update(cell);
                        }
                    }
                    var responseField = ConvertField(result.Field);
                    await Clients.User(userId).SendAsync("PlaceShipResult", "Placed", responseField);
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
        public async Task Shoot(string gameId, string x,string y)
        {
            _cache.TryGetValue(gameId, out Game game);
            var CurrId = Context.UserIdentifier;
            var player = game.GameBoards.FirstOrDefault(p => p.Player.User.Id == CurrId).Player;
            var userId = game.GameBoards.FirstOrDefault(b => b.Player.Id != player.Id).Player.User.Id;
            try
            {
                var ShootedCell = game.Shoot(player,new Coords(int.Parse(x),int.Parse(y)));
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

        public async Task Ready(string gameId)
        {
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
                    await Clients.User(startId).SendAsync("Your_Turn");
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
