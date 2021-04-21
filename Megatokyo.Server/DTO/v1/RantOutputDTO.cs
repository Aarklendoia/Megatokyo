using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.DTO.v1
{
    public class RantOutputDTO
    {
        public int RantId { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }
        public Uri Url { get; set; }
        public DateTime DateTime { get; set; }
        public string Content { get; set; }
    }
}
