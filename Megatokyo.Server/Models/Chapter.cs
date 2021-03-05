using Megatokyo.Server.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.Models
{
    public class Chapter
    {
        public int ChapterId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
    }

    public class DetailedChapter : Chapter
    {
        public List<Strip> Strips { get; } = new List<Strip>();

        internal void LoadStrips(IEnumerable<Strips> strips)
        {
            foreach (Strips strip in strips)
            {
                Strips.Add(new Strip
                {
                    ChapterId = strip.ChapterId,
                    Date = strip.Date,
                    Number = strip.Number,
                    StripId = strip.StripId,
                    Title = strip.Title,
                    Url = strip.Url
                });
            }
        }
    }
}
