using System.Collections.Generic;

namespace Megatokyo.Server.DTO.v1
{
    internal class ChapterOutputDTO
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
    }
}