using System;

namespace Megatokyo.Server.Database.Models
{
    public partial class Checking
    {
        public int ChekingId { get; set; }
        public DateTime LastCheck { get; set; }
        public int LastStripNumber { get; set; }
        public int LastRantNumber { get; set; }
    }
}