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
            //return Redirect("index");
            return View();
        }
    }
}
