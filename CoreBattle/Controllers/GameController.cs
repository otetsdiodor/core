using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CoreBattle.Controllers
{
    public class GamesController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.gameId = TempData["gameId"];
            return View();
        }
    }
}
