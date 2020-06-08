using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace intrapp.Models
{
    public class ParticipantIdentity
    {
        public int ParticipantId { get; set; }
        public Player Player { get; set; }
    }
}