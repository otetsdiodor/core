using CoreBattle.Domain.Core.GameDomain;
using CoreBattle.Domain.Core.ManageDomain;
using CoreBattle.Domain.Interfaces;
using CoreBattle.Helpers;
using CoreBattle.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBattle.Controllers
{
    [ServiceFilter(typeof(GameHubInit))]
    [Authorize]
    public class GameslistController : Controller
    {
        IRepository<Game> _gameRepository;
        IRepository<GameBoard> _gBRepository;
        IRepository<Player> _playerRepository;
        private readonly UserManager<User> _userManager;
        private IMemoryCache _cache;

        public GameslistController(IRepository<Game> repository, UserManager<User> userManager, IRepository<Player> playerRepository, IMemoryCache cache, IRepository<GameBoard> gBRepository)
        {
            _gameRepository = repository;
            _gBRepository = gBRepository;
            _userManager = userManager;
            _playerRepository = playerRepository;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Games = _gameRepository.GetAll().Where(g => g.Status == GameStatus.Waiting);
            return View();
        }

        [HttpPost]
        public IActionResult CreateGame()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var player = _playerRepository.GetAll().Include(p => p.User).Where(p => p.User.Id == user.Id).FirstOrDefault();

            if (player == null)
            {
                player = new Player(user);
                _playerRepository.Insert(player);
            }

            var g = new Game(10, player);
            _cache.Set(g.Id.ToString(), g,new MemoryCacheEntryOptions { 
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
            _cache.Set(user.Id, g.Id.ToString());
            _gameRepository.Insert(g);
            TempData["gameId"] = g.Id; 
            return RedirectToAction("Index","Games");
        }

        [HttpPost]
        public IActionResult ConnectToGame(string gameId)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var player = _playerRepository.GetAll().Include(p => p.User).Where(p => p.User.Id == user.Id).FirstOrDefault();

            _cache.TryGetValue(gameId, out Game game);
            if (player == null)
            {
                player = new Player(user);
                _playerRepository.Insert(player);
            }

            game.AddBoard(player);
            game.Status = GameStatus.GoesOn;

            try
            {
                game.GameBoards[1].GameId = game.Id;
                _gBRepository.Insert(game.GameBoards[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e. Message);
            }

            var g = _gameRepository.Get(Guid.Parse(gameId));
            g.Status = GameStatus.GoesOn;
            _gameRepository.Update(g);

            _cache.Remove(gameId);
            _cache.Set(game.Id.ToString(), game, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
            _cache.Set(user.Id, g.Id.ToString());
            TempData["gameId"] = game.Id;
            return RedirectToAction("Index", "Games");
        }
    }
}
