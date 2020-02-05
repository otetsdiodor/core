using System;
using System.Collections.Generic;
using System.Linq;
using CoreBattle.Domain.Core.GameDomain;
using CoreBattle.Domain.Interfaces;
using CoreBattle.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreBattle.Controllers
{
    [Route("api/[controller]")]
    public class StatisticController : Controller
    {
        IRepository<ResultStats> _statRepository;
        
        public StatisticController(IRepository<ResultStats> statsRepo)
        {
            _statRepository = statsRepo;
        }

        [HttpGet]
        public IEnumerable<ResultStats> Get(FilterModel filter)
        {
            var stat = _statRepository.GetAll().Include(r => r.ShipsInfo).AsQueryable();
            if (filter.Name != null && filter.Name != "")
                stat = stat.Where(s => s.WinnerName == filter.Name);

            if (filter.Date != null && filter.Date != "")
                if (DateTime.TryParse(filter.Date,out DateTime date))
                    stat = stat.Where(s => s.EndTime.Year == date.Year && s.EndTime.Day == date.Day && s.EndTime.Month == s.EndTime.Month);

            stat = stat.Where(s => s.CountOfSteps > filter.CountSteps)
                       .Where(s => s.ShipsInfo.Count() > filter.CountShips);

            switch ((SortState)filter.State)
            {
                case SortState.NameAsc: stat = stat.OrderBy(s => s.WinnerName);
                    break;
                case SortState.NameDesc: stat =stat.OrderByDescending(s => s.WinnerName);
                    break;
                case SortState.ShipAsc:
                    stat = stat.OrderBy(s => s.ShipsInfo.Count);
                    break;
                case SortState.ShipDesc:
                    stat = stat.OrderByDescending(s => s.ShipsInfo.Count);
                    break;
                case SortState.StepsAsc:
                     stat =stat.OrderBy(s => s.CountOfSteps);
                    break;
                case SortState.StepsDesc:
                    stat = stat.OrderByDescending(s => s.CountOfSteps);
                    break;
                case SortState.DateAsc:
                    stat = stat.OrderBy(s => s.EndTime);
                    break;
                case SortState.DateDesc:
                    stat = stat.OrderByDescending(s => s.EndTime);
                    break;
            }
            return stat.ToList();
        }
    }
}
