using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class MatchBreakdown
    {
        public long GameId { get; set; }
        public string PlatformId { get; set; }
        public long GameCreation { get; set; }
        public int GameDuration { get; set; }
        public int QueueId { get; set; }
        public int MapId { get; set; }
        public int SeasonId { get; set; }
        public string GameVersion { get; set; }
        public string GameMode { get; set; }
        public string GameType { get; set; }
        public IList<Team> Teams { get; set; }
        public IList<Participant> Participants { get; set; }
        public IList<ParticipantIdentity> ParticipantIdentities { get; set; }


        public IEnumerable<IGrouping<int, Participant>> ParticipantsByTeam { get; set; }
        public IList<ParticipantForDisplay> ParticipantsForDisplay { get; set; }
        public IEnumerable<IGrouping<int, ParticipantForDisplay>> ParticipantsForDisplayByTeam { get; set; }
        public string QueueTypeName { get; set; }
        public TeamsBreakdown TeamsBreakdown { get; set; }
        public int HighestDamageDealtToChampionsByAParticipant { get; set; }

        public MatchBreakdown()
        {
            ParticipantsForDisplay = new List<ParticipantForDisplay>();
        }

    }
}