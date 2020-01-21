using CoreBattle.Domain.Core.GameDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBattle.Models
{
    public class StatViewModel
    {
        public List<ResultStats> Stats { get; set; }
        public string NickName { get; set; }
        public int CountOfShips { get; set; }
        public int CountOfSteps { get; set; }
        public DateTime date { get; set; }
    }
}
