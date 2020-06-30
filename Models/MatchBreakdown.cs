using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class MatchBreakdown
    {
        public Match Match { get; set; }
        public Participant ObservedParticipant { get; set; }
        public IEnumerable<IGrouping<int, Participant>> ParticipantsByTeam { get; set; }
        public IList<ParticipantForDisplay> ParticipantsForDisplay { get; set; }
        public IEnumerable<IGrouping<int, ParticipantForDisplay>> ParticipantsForDisplayByTeam { get; set; }
        public TeamsBreakdown TeamsBreakdown { get; set; }
        public int HighestDmgDealt { get; set; }
        public int HighestKills { get; set; }
        public int HighestGold { get; set; }
        public int HighestWardsPlaced { get; set; }
        public int HighestDmgTaken { get; set; }
        public int HighestCS { get; set; }
        public EventsTimeline Timeline { get; set; }
        public List<RunePath> Runes { get; set; }
        public List<Spell> Spells { get; set; }

        public MatchBreakdown()
        {
            ParticipantsForDisplay = new List<ParticipantForDisplay>();
        }

    }
}