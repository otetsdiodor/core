using CoreBattle.Domain.Core.GameDomain;
using CoreBattle.Domain.Core.ManageDomain;
using CoreBattle.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public GameslistController(Repository<Game> repository, UserManager<User> userManager, Repository<Player> playerRepository)
        {
            _gameRepository = repository;
            _userManager = userManager;
            _playerRepository = playerRepository;
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
                player = new Player(user);

            var g = new Game(10, player);
            var player2 = new Player(user);
            g.AddBoard(player2);
            g.GameBoards[0].PlaceShip(new Coords(0, 0), new Coords(0, 3));
            g.GameBoards[1].PlaceShip(new Coords(0, 0), new Coords(0, 3));
            //g.GameBoards[0].Shoot(new Coords(1, 0));
            g.Shoot(player2, new Coords(0, 0));
            g.Shoot(player2, new Coords(0, 9));
            g.Shoot(player, new Coords(0, 1));
            g.Shoot(player2, new Coords(0, 2));
            g.Shoot(player, new Coords(0, 3));
            g.Shoot(player2, new Coords(0, 1));
            g.Shoot(player, new Coords(0, 5));
            g.Shoot(player2, new Coords(0, 3));
            //g.GameBoards[0].PlaceShip(new Coords(2, 0), new Coords(2, 2));
            _gameRepository.Insert(g);
            return RedirectToAction("Index","Gameslist");
        }

        [HttpPost]
        public async Task<IActionResult> ConnectToGame(string gameId)
        {
            var user = await _userManager.GetUserAsync(User);
            var player = _playerRepository.GetAll().Include(p => p.User).Where(p => p.User.Id == user.Id).FirstOrDefault();

            var game = _gameRepository.GetAll().Include(g => g.GameBoards).Where(g => g.Id == Guid.Parse(gameId)).FirstOrDefault(); ;

            if (player == null)
                player = new Player(user);

            game.AddBoard(player);
            return RedirectToAction("Index", "Gameslist");
        }
    }
}
