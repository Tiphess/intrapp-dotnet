using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class Timeline
    {
        public int ParticipantId { get; set; }
        public CreepsPerMinDeltas CreepsPerMinDeltas { get; set; }
        public XpPerMinDeltas XpPerMinDeltas { get; set; }
        public GoldPerMinDeltas GoldPerMinDeltas { get; set; }
        public CsDiffPerMinDeltas CsDiffPerMinDeltas { get; set; }
        public XpDiffPerMinDeltas XpDiffPerMinDeltas { get; set; }
        public DamageTakenPerMinDeltas DamageTakenPerMinDeltas { get; set; }
        public DamageTakenDiffPerMinDeltas DamageTakenDiffPerMinDeltas { get; set; }
        public string Role { get; set; }
        public string Lane { get; set; }
    }

    public class CreepsPerMinDeltas
    {
        public Dictionary<string, double> Data { get; set; }

        public CreepsPerMinDeltas()
        {
            Data = new Dictionary<string, double>();
        }
    }

    public class XpPerMinDeltas
    {
        public Dictionary<string, double> Data { get; set; }

        public XpPerMinDeltas()
        {
            Data = new Dictionary<string, double>();
        }
    }

    public class GoldPerMinDeltas
    {
        public Dictionary<string, double> Data { get; set; }

        public GoldPerMinDeltas()
        {
            Data = new Dictionary<string, double>();
        }
    }

    public class CsDiffPerMinDeltas
    {
        public Dictionary<string, double> Data { get; set; }

        public CsDiffPerMinDeltas()
        {
            Data = new Dictionary<string, double>();
        }
    }

    public class XpDiffPerMinDeltas
    {
        public Dictionary<string, double> Data { get; set; }

        public XpDiffPerMinDeltas()
        {
            Data = new Dictionary<string, double>();
        }
    }

    public class DamageTakenPerMinDeltas
    {
        public Dictionary<string, double> Data { get; set; }

        public DamageTakenPerMinDeltas()
        {
            Data = new Dictionary<string, double>();
        }
    }

    public class DamageTakenDiffPerMinDeltas
    {
        public Dictionary<string, double> Data { get; set; }

        public DamageTakenDiffPerMinDeltas()
        {
            Data = new Dictionary<string, double>();
        }
    }
}