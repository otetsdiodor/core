using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace CoreBattle.Domain.Core.GameDomain
{
    public class ResultStats : Entity
    {
        public string WinnerName { get; set; }
        public int CountOfSteps { get; set; }
        public List<ShipInfo> ShipsInfo { get; set; }
        [Column(TypeName = "date")]
        public DateTime EndTime { get; set; }
        public ResultStats()
        {

        }
        public ResultStats(Player winner, Game g)
        {
            ShipsInfo = new List<ShipInfo>();
            WinnerName = winner.User.NickName;
            CountOfSteps = g.GameHistory.Count;
            var count = 0;
            EndTime = DateTime.Now;
            foreach (var ship in g.GameBoards.FirstOrDefault(b => b.PlayerId == winner.Id).Ships)
            {
                foreach (var c in ship.Coords)
                {
                    foreach (var row in g.GameBoards.FirstOrDefault(b => b.PlayerId == winner.Id).Field)
                    {
                        foreach (var cell in row.CellsRow)
                        {
                            if (c.X == cell.X && c.Y == cell.Y && cell.State == CellState.Ship)
                            {
                                count++;
                            }
                        }
                    }
                }
                ShipsInfo.Add(new ShipInfo(ship.Length, count));
                count = 0;
            }
        }
    }
}
