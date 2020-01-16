using CoreBattle.Domain.Core.GameDomain;
using CoreBattle.Domain.Core.ManageDomain;
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
    [Authorize]
    public class GameslistController : Controller
    {
        Repository<Game> _gameRepository;
        Repository<Player> _playerRepository;
        private readonly UserManager<User> _userManager;
        private IMemoryCache _cache;

        public GameslistController(Repository<Game> repository, UserManager<User> userManager, Repository<Player> playerRepository, IMemoryCache cache)
        {
            _gameRepository = repository;
            _userManager = userManager;
            _playerRepository = playerRepository;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var lol =  _gameRepository.GetAll().Where(g => g.Status == GameStatus.Waiting);
            lol.Load();
            ViewBag.Games = lol;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame()
        {
            var user = await _userManager.GetUserAsync(User);
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
            _gameRepository.Insert(g);
            TempData["gameId"] = g.Id; 
            return RedirectToAction("Index","Games");
        }

        [HttpPost]
        public async Task<IActionResult> ConnectToGame(string gameId)
        {
            var user = await _userManager.GetUserAsync(User);
            var player = _playerRepository.GetAll().Include(p => p.User).Where(p => p.User.Id == user.Id).FirstOrDefault();

            //var game = _gameRepository.GetAll().Include(g => g.GameBoards).Where(g => g.Id == Guid.Parse(gameId)).FirstOrDefault(); ;
            _cache.TryGetValue(gameId, out Game game);
            if (player == null)
            {
                player = new Player(user);
                _playerRepository.Insert(player);
            }

            game.AddBoard(player);
            game.Status = GameStatus.GoesOn;
            _cache.Remove(gameId);
            _cache.Set(game.Id.ToString(), game, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });
            TempData["gameId"] = game.Id;
            return RedirectToAction("Index", "Games");
        }
    }
}
