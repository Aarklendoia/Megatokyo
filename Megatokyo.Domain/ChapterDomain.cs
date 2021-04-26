using System;
using System.Collections.Generic;

namespace Megatokyo.Domain
{
    public class ChapterDomain
    {
        public int Number { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }

        public ChapterDomain(int number, string title, string category)
        {
            Number = number;
            Title = title;
            Category = category;
        }
    }
}
