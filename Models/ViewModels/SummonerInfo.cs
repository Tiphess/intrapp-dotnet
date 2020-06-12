using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models.ViewModels
{
    public class SummonerInfo
    {
        public Summoner Summoner { get; set; }
        public MatchHistory MatchHistory { get; set; }
        public List<LeagueEntry> LeagueEntries { get; set; }
        public string ProfileIconUrl { get; set; }
        public string Region { get; set; }
        public string LastPlayed { get; set; }
    }
}