using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class Participant
    {
        public int ParticipantId { get; set; }
        public int TeamId { get; set; }
        public int ChampionId { get; set; }
        public int Spell1Id { get; set; }
        public int Spell2Id { get; set; }
        public Stats Stats { get; set; }
        public Timeline Timeline { get; set; }
        public Player Player { get; set; }
        public string DisplayedSummonerName { get; set; }
        public string ChampionPlayedIcon { get; set; }
    }
}