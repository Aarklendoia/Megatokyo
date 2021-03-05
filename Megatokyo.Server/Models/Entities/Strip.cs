using Megatokyo.Server.Database.Models;
using System;

namespace Megatokyo.Server.Models.Entities
{
    public class Strip
    {
        public string Title { get; set; }
        public int Number { get; set; }
        public string Category { get; set; }
        public Uri Url { get; set; }
        public DateTime DateTime { get; set; }
        public Chapter Chapter { get; set; }

        public Strip()
        {
            Title = "";
            Number = 0;
            Category = "";
            Url = null;
            DateTime = DateTime.MinValue;
            Chapter = null;
        }

        public Strip(Strips stripFromDatabase)
        {
            if (stripFromDatabase == null)
            {
                throw new ArgumentNullException(nameof(stripFromDatabase));
            }

            Title = stripFromDatabase.Title;
            Number = stripFromDatabase.Number;
            //Category = stripFromDatabase.Chapter.Category;
            Url = stripFromDatabase.Url;
            DateTime = stripFromDatabase.Date;
            //Chapter = new Chapter(stripFromDatabase.Chapter);
        }
    }
}
