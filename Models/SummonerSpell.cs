using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class SummonerSpell
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SummonerLevel { get; set; }
        public int Cooldown { get; set; }
        public IList<string> GameModes { get; set; }
        public string IconPath { get; set; }
    }
}