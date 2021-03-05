using Megatokyo.Server.Database.Models;
using System;

namespace Megatokyo.Server.Models.Entities
{
    public class Chapter
    {
        public string Title { get; set; }
        public int Number { get; set; }
        public string Category { get; set; }

        public Chapter()
        {
            Title = "";
            Number = 0;
            Category = "";
        }

        public Chapter(Chapters chapterFromDatabase)
        {
            if (chapterFromDatabase == null)
            {
                throw new ArgumentNullException(nameof(chapterFromDatabase));
            }

            Title = chapterFromDatabase.Title;
            Number = chapterFromDatabase.Number;
            Category = chapterFromDatabase.Category;
        }
    }
}
