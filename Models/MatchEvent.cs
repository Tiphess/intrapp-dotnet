using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class MatchEvent
    {
        //Legal values for Type property
        public static readonly List<string> LEGAL_VALUES = new List<string> { "CHAMPION_KILL", "WARD_PLACED", "WARD_KILL", "BUILDING_KILL", "ELITE_MONSTER_KILL",
                                                                              "ITEM_PURCHASED", "ITEM_SOLD", "ITEM_DESTROYED", "ITEM_UNDO", "SKILL_LEVEL_UP" };
        public string Type { get; set; }
        public long Timestamp { get; set; }
        public int ParticipantId { get; set; }
        public int ItemId { get; set; }
        public string LaneType { get; set; }
        public int SkillSlot { get; set; }
        public string AscendedType { get; set; }
        public int CreatorId { get; set; }
        public int AfterId { get; set; }
        public string EventType { get; set; }
        public string LevelUpType { get; set; }
        public string WardType { get; set; }
        public string TowerType { get; set; }
        public int BeforeId { get; set; }
        public string PointCaptured { get; set; }
        public string MonsterType { get; set; }
        public string MonsterSubType { get; set; }
        public int TeamId { get; set; }
        public MatchPosition Position { get; set; }
        public int KillerId { get; set; }
        public List<int> AssistingParticipantIds { get; set; }
        public string BuildingType { get; set; }
        public int VictimId { get; set; }

        public string ItemUrl { get; set; }
    }

    public class MatchPosition
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}