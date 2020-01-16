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
        private Repository<Game> _gameRepository;

        public GameHub(IMemoryCache cache, UserManager<User> userManager, Repository<Player> playerRepository, Repository<Game> gameRepository)
        {
            _gameRepository = gameRepository;
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
            _cache.TryGetValue(gameId,out Game game);
            var user = await _userManager.GetUserAsync(Context.User);
            var player = _playerRepository.GetAll().Include(p => p.User).Where(p => p.User.Id == user.Id).FirstOrDefault();
            //var userId = game.GameBoards.FirstOrDefault(b => b.Player != player).Player.Id;

            try
            {
                var result = game.GameBoards.FirstOrDefault(b => b.Player.Id == player.Id);
                result.PlaceShip(new Coords(int.Parse(x1), int.Parse(y1)), new Coords(int.Parse(x2), int.Parse(y2)));
                await Clients.User(user.Id.ToString()).SendAsync("PlaceShipResult", "Placed",result.Field);
            }
            catch (Exception e)
            {
                await Clients.User(user.Id).SendAsync("ErrorHandler", e.Message);
            }
            _cache.Remove(game.Id);
            _cache.Set(game.Id, game);
        }

        public async Task Shoot(string gameId, string x,string y)
        {
            _cache.TryGetValue(gameId, out Game game);
            var user = await _userManager.GetUserAsync(Context.User);
            var player = _playerRepository.GetAll().Include(p => p.User).Where(p => p.User.Id == user.Id).FirstOrDefault();
            var userId = game.GameBoards.FirstOrDefault(b => b.Player.Id != player.Id).Player.User.Id;
            try
            {
                game.Shoot(player,new Coords(int.Parse(x),int.Parse(y)));
                var my = game.GameBoards.FirstOrDefault(b => b.Player.Id == player.Id).Field;
                var enemy = game.GameBoards.FirstOrDefault(b => b.Player.Id != player.Id).Field;
                await Clients.User(user.Id).SendAsync("ShootResult", my,enemy);
                await Clients.User(userId).SendAsync("ShootResult", enemy,my);
                if (game.IsGameEnded(player))
                {
                    await Clients.User(user.Id).SendAsync("EndGame", "WIN");
                    await Clients.User(userId).SendAsync("EndGame", "LOSE");
                    _gameRepository.Update(game);
                }
            }
            catch (Exception e)
            {
                await Clients.User(user.Id).SendAsync("ErrorHandler", e.Message);
            }
            _cache.Remove(game.Id);
            _cache.Set(game.Id.ToString(), game);
        }

        public async Task Ready(string gameId)
        {
            _cache.TryGetValue(gameId, out Game game);
            var user = await _userManager.GetUserAsync(Context.User);
            var player = _playerRepository.GetAll().Include(p => p.User).Where(p => p.User.Id == user.Id).FirstOrDefault();
            var userId = game.GameBoards.FirstOrDefault(b => b.Player.Id != player.Id).Player.User.Id;
            try
            {
                game.GameBoards.FirstOrDefault(b => b.Player.Id == player.Id).IsReady = true;
                if (game.IsValidToStart())
                {
                    //var my = game.GameBoards.FirstOrDefault(b => b.Player.Id == player.Id).Field;
                    //var enemy = game.GameBoards.FirstOrDefault(b => b.Player.Id != player.Id).Field;
                    await Clients.Users(user.Id,userId.ToString()).SendAsync("START_GAME"/*, my, enemy*/);
                    //await Clients.User(userId.ToString()).SendAsync("START_GAME"/*, enemy, my*/);
                }
            }
            catch (Exception e)
            {
                await Clients.User(user.Id).SendAsync("ErrorHandler", e.Message);
            }
            
            _cache.Remove(game.Id);
            _cache.Set(game.Id.ToString(), game);

            await Clients.User(user.Id).SendAsync("ReadyResult", "OK");
        }
    }
}
