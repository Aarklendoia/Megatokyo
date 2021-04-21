using System;
using System.Collections.Generic;

namespace Megatokyo.Domain
{
    public class ChapterDomain
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public List<StripDomain> Strips { get; set; }

        public ChapterDomain(int id, int number, string title, string category)
        {
            Id = id;
            Number = number;
            Title = title;
            Category = category;
            Strips = new();
        }

        public ChapterDomain(int id, int number, string title, string category, IEnumerable<StripDomain> strips)
        {
            Id = id;
            Number = number;
            Title = title;
            Category = category;
            Strips = new();
            Strips.AddRange(strips);
        }
    }
}
