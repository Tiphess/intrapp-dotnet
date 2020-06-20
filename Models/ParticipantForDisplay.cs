using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class ParticipantForDisplay
    {
        public string ChampionIconUrl { get; set; }
        public string SummonerSpell1IconUrl { get; set; }
        public string SummonerSpell2IconUrl { get; set; }
        public string RuneKeystoneIconUrl { get; set; }
        public string RuneSecondaryPathIconUrl { get; set; }
        public Participant Participant { get; set; }
    }
}