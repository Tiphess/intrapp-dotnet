using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class League
    {
        public readonly static int PageSize = 100;

        public string Tier { get; set; }
        public string LeagueId { get; set; }
        public string Queue { get; set; }
        public string Name { get; set; }
        public IList<Entry> Entries { get; set; }
        public int Pages { get; set; }
        public int? CurrentPage { get; set; }


        public League()
        {
            Entries = new List<Entry>();
        }
        public League(League obj)
        {
            Tier = obj.Tier;
            LeagueId = obj.LeagueId;
            Queue = obj.Queue;
            Name = obj.Name;
            Pages = obj.Pages;
            CurrentPage = obj.CurrentPage;

            Entries = new List<Entry>(obj.Entries);
        }
    }

    public class Entry
    {
        public string SummonerId { get; set; }
        public string SummonerName { get; set; }
        public int LeaguePoints { get; set; }
        public string Rank { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public bool Veteran { get; set; }
        public bool Inactive { get; set; }
        public bool FreshBlood { get; set; }
        public bool HotStreak { get; set; }
    }
}