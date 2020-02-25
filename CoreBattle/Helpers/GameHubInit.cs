using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBattle.Helpers
{
    public class GameHubInit : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var item in context.HttpContext.User.Claims)
            {
                Console.WriteLine(item.Value);
            }
        }
    }
}
