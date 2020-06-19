using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class Rune
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string LongDesc { get; set; }
    }

    public class Slot
    {
        public IList<Rune> Runes { get; set; }
    }

    public class RunePath
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public IList<Slot> Slots { get; set; }
    }
}