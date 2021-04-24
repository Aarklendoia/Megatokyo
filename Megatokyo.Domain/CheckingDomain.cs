using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megatokyo.Domain
{
    public class CheckingDomain
    { 
        public int Id { get; set; }
        public DateTime LastCheck { get; set; }
        public int LastStripNumber { get; set; }
        public int LastRantNumber { get; set; }

        public CheckingDomain(DateTime lastCheck, int lastRantNumber, int lastStripNumber)
        {
            LastCheck = lastCheck;
            LastRantNumber = lastRantNumber;
            LastStripNumber = lastStripNumber;
        }
    }
}
