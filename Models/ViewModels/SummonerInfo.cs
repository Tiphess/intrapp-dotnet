using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models.ViewModels
{
    public class SummonerInfo
    {
        public Summoner Summoner { get; set; }
        public List<Match> MatchHistory { get; set; }
        public List<LeagueEntry> LeagueEntries { get; set; }
        public string ProfileIconUrl { get; set; }
    }
}