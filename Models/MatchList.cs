using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class MatchList
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public int TotalGames { get; set; }
        public List<MatchReference> Matches { get; set; }
    }
}