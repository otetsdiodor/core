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

namespace CoreBattle.Controllers
{
    public class GameHub : Hub
    {
        private IMemoryCache _cache;
        private IRepository<ResultStats> _statRepository;
        private IRepository<Game> _gameRepository;
        private IRepository<Cell> _cellRepository;
        private IRepository<Ship> _shipRepository;
        private IRepository<StepHistory> _stepRepository;
        private IRepository<GameBoard> _gBRepository;

        private Game game;
        private Player CurrentPlayer;
        private Player OponentPlayer;

        public GameHub(IMemoryCache cache,
            IRepository<ResultStats> statRepository,
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
        }

        public async override Task OnConnectedAsync()
        {
            InitializeHub();
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
            _cache.TryGetValue(Context.UserIdentifier, out string gameId);
            _cache.TryGetValue(gameId, out game);
            CurrentPlayer = game.GameBoards.FirstOrDefault(p => p.Player.User.Id == Context.UserIdentifier).Player;
            OponentPlayer = game.GameBoards.FirstOrDefault(b => b.Player.Id != CurrentPlayer.Id)?.Player;
        }
        public async Task PlaceShip(int x1, int y1, int x2, int y2)
        {
            InitializeHub();
            try
            {
                var result = game.GameBoards.FirstOrDefault(b => b.Player.Id == CurrentPlayer.Id);
                var ship = result.PlaceShip(new Coords(x1, y1), new Coords(x2, y2));
                _shipRepository.Insert(ship);
                foreach (var item in result.Field)
                    foreach (var cell in item.CellsRow)
                        _cellRepository.Update(cell);

                var responseField = ConvertField(result.Field);
                await Clients.User(Context.UserIdentifier).SendAsync("PlaceShipResult", responseField);
            }
            catch (Exception e)
            {
                await Clients.User(Context.UserIdentifier).SendAsync("ErrorHandler", e.Message);
            }
            _cache.Remove(game.Id);
            _cache.Set(game.Id.ToString(), game);
        }

        private List<object> ConvertField(List<Row> field)
        {
            var result = new List<object>();
            foreach (var item in field)
                foreach (var cell in item.CellsRow)
                    result.Add(new { cell.X, cell.Y, cell.State });

            return result;
        }
        public async Task Shoot(int x,int y)
        {
            InitializeHub();
            try
            {
                var ShootedCell = game.Shoot(CurrentPlayer,new Coords(x,y));
                ShootedCell.Row = null;
                _cellRepository.Update(ShootedCell);
                _stepRepository.Insert(game.GameHistory.Last());
                var myField = ConvertField(game.GameBoards.FirstOrDefault(b => b.Player.Id == CurrentPlayer.Id).Field);
                var enField = ConvertField(game.GameBoards.FirstOrDefault(b => b.Player.Id != CurrentPlayer.Id).Field);
                await Clients.User(CurrentPlayer.User.Id).SendAsync("ShootResult", myField, enField);
                await Clients.User(OponentPlayer.User.Id).SendAsync("ShootResult", enField, myField);
                if (game.IsGameEnded(CurrentPlayer))
                {
                    var g = _gameRepository.Get(game.Id);
                    g.Status = GameStatus.Completed;
                    _gameRepository.Update(g);
                    _statRepository.Insert(new ResultStats(CurrentPlayer, game));
                    await Clients.User(CurrentPlayer.User.Id).SendAsync("EndGame", "WIN");
                    await Clients.User(OponentPlayer.User.Id).SendAsync("EndGame", "LOSE");
                }
            }
            catch (Exception e)
            {
                await Clients.User(CurrentPlayer.User.Id).SendAsync("ErrorHandler", e.Message);
            }
            _cache.Remove(game.Id);
            _cache.Set(game.Id.ToString(), game);
        }

        public async Task Ready()
        {
            InitializeHub();
            try
            {
                var board = game.GameBoards.FirstOrDefault(b => b.Player.Id == CurrentPlayer.Id);
                board.IsReady = true;
                _gBRepository.Update(board);
                if (game.IsValidToStart())
                {
                    var startId = game.Current.User.Id;
                    await Clients.User(startId).SendAsync("ErrorHandler", "YOUR_TURN");
                    await Clients.Users(CurrentPlayer.User.Id, OponentPlayer.User.Id).SendAsync("START_GAME");
                }
            }
            catch (Exception e)
            {
                await Clients.User(CurrentPlayer.User.Id).SendAsync("ErrorHandler", e.Message);
            }
            
            _cache.Remove(game.Id);
            _cache.Set(game.Id.ToString(), game);

            await Clients.User(CurrentPlayer.User.Id).SendAsync("ReadyResult", "OK");
        }
    }
}
