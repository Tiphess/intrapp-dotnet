using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class Player
    {
        public string PlatformId { get; set; }
        public string AccountId { get; set; }
        public string SummonerName { get; set; }
        public string SummonerId { get; set; }
        public string CurrentPlatformId { get; set; }
        public string CurrentAccountId { get; set; }
        public string MatchHistoryUri { get; set; }
        public int ProfileIcon { get; set; }
    }
}