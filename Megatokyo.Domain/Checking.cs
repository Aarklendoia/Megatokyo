using System;

namespace Megatokyo.Domain
{
    public class Checking
    {
        public int Id { get; set; }
        public DateTimeOffset LastCheck { get; set; }
        public int LastStripNumber { get; set; }
        public int LastRantNumber { get; set; }

        public Checking(DateTimeOffset lastCheck, int lastRantNumber, int lastStripNumber)
        {
            LastCheck = lastCheck;
            LastRantNumber = lastRantNumber;
            LastStripNumber = lastStripNumber;
        }
    }
}
