using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class TeamsBreakdown
    {
        public int BlueTeamBaronKills { get; set; }
        public int BlueTeamDragonKills { get; set; }
        public int BlueTeamTowerKills { get; set; }
        public int RedTeamBaronKills { get; set; }
        public int RedTeamDragonKills { get; set; }
        public int RedTeamTowerKills { get; set; }
        public int BlueTeamChampionKills { get; set; }
        public int RedTeamChampionKills { get; set; }
        public int BlueTeamGold { get; set; }
        public int RedTeamGold { get; set; }
    }
}