using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBattle.Domain.Core.GameDomain;
using CoreBattle.Infrastructure.Data;
using CoreBattle.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoreBattle.Controllers
{
    [Route("api/[controller]")]
    public class StatisticController : Controller
    {
        Repository<ResultStats> _statRepository;
        
        public StatisticController(Repository<ResultStats> statsRepo)
        {
            _statRepository = statsRepo;
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<ResultStats> Get(FilterModel filter)
        {
            var state = (SortState)filter.State;

            var stat = _statRepository.GetAll().Include(r => r.ShipsInfo).AsQueryable();
            if (filter.Name != null && filter.Name != "")
            {
                stat = stat.Where(s => s.WinnerName == filter.Name);
            }
            if (filter.Date != null && filter.Date != "")
            {
                var date = DateTime.Parse(filter.Date);
                stat = stat.Where(s => s.EndTime.Year == date.Year && s.EndTime.Day == date.Day && s.EndTime.Month == s.EndTime.Month);
            }
            if (filter.CountSteps != 0)
            {
                stat = stat.Where(s => s.CountOfSteps > filter.CountSteps);
            }
            if (filter.CountShips != 0)
            {
                stat = stat.Where(s => s.ShipsInfo.Count() > filter.CountShips);
            }

            switch (state)
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
                    stat = stat.OrderByDescending(s => s.EndTime);
                    break;
                case SortState.DateDesc:
                    stat = stat.OrderByDescending(s => s.EndTime);
                    break;
            }
            return stat.ToList();
        }
    }
}
