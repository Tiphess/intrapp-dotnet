using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class EventsTimeline
    {
        public List<MatchEvent> ItemPurchaseEvents { get; set; }
        public Dictionary<long, List<MatchEvent>> ItemPurchaseEventsByTimestamp { get; set; }
        public List<MatchEvent> SkillEvents { get; set; }

        public EventsTimeline()
        {
            ItemPurchaseEvents = new List<MatchEvent>();
            SkillEvents = new List<MatchEvent>();
            ItemPurchaseEventsByTimestamp = new Dictionary<long, List<MatchEvent>>();
        }
    }
}