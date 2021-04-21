using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.DTO.v1
{
    public class StripOutputDTO
    {
        public ChapterOutputDTO Chapter { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public Uri Url { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
    }
}
