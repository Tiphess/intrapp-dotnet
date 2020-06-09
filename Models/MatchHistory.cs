using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class MatchHistory
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
        public List<Match> Matches { get; set; }

        public MatchHistory()
        {
            Matches = new List<Match>();
        }
    }
}