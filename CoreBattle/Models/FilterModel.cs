using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBattle.Models
{
    public enum SortState
    {
        Default=-1,
        NameAsc,   
        NameDesc,  
        ShipAsc, 
        ShipDesc,   
        StepsAsc,
        StepsDesc,
        DateAsc,
        DateDesc
    }

    public class FilterModel
    {
        public string Name { get; set; }
        public int CountShips { get; set; }
        public int CountSteps { get; set; }
        public string Date { get; set; }
        public int State { get; set; }
    }
}
