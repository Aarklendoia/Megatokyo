using Megatokyo.Server.Database.Models;
using System;
using System.Collections.Generic;

namespace Megatokyo.Server.Models
{
    public class Strip
    {
        public int StripId { get; set; }
        public int ChapterId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public Uri Url { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
    }

    public class DetailedStrip : Strip
    {
        public Chapter Chapter { get; set; } = new Chapter();

        public DetailedStrip()
        {
        }

        public DetailedStrip(Strip strip)
        {
            if (strip != null)
            {
                StripId = strip.StripId;
                ChapterId = strip.ChapterId;
                Number = strip.Number;
                Title = strip.Title;
                Url = strip.Url;
                Date = strip.Date;
                Category = strip.Category;
            }
        }

        public DetailedStrip(Strips strip)
        {
            if (strip != null)
            {
                StripId = strip.StripId;
                ChapterId = strip.ChapterId;
                Number = strip.Number;
                Title = strip.Title;
                Url = strip.Url;
                Date = strip.Date;
            }
        }

        internal void LoadChapter(Chapters chapter)
        {
            Chapter.Category = chapter.Category;
            Chapter.ChapterId = chapter.ChapterId;
            Chapter.Number = chapter.Number;
            Chapter.Title = chapter.Title;
            Category = chapter.Category;
        }
    }
}