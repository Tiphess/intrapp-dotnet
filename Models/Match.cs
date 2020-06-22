using intrapp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class Match
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


        public long Timestamp { get; set; }
        public string WasPlayed { get; set; }
        public IEnumerable<IGrouping<int, Participant>> ParticipantsByTeam { get; set; }
        public string QueueTypeName { get; set; }
        public string GameDurationStr { get; set; }
        public string GameResult { get; set; }
        public ParticipantForDisplay ParticipantForDisplay { get; set; }
        //public string TierAverage { get; set; }
    }
}