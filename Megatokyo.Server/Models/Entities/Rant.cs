using Megatokyo.Server.Database.Models;
using System;

namespace Megatokyo.Server.Models.Entities
{
    public class Rant
    {
        public string Title { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }
        public Uri Url { get; set; }
        public DateTime DateTime { get; set; }
        public string Content { get; set; }

        public Rant()
        {
            Title = "";
            Number = 0;
            Author = "";
            Url = null;
            DateTime = DateTime.MinValue;
            Content = "";
        }

        public Rant(Rants rantFromDatabase)
        {
            if (rantFromDatabase == null)
            {
                throw new ArgumentNullException(nameof(rantFromDatabase));
            }

            //Title = rantFromDatabase.Title;
            Number = rantFromDatabase.Number;
            Author = rantFromDatabase.Author;
            Url = null;
            DateTime = rantFromDatabase.Date;
            Content = "";
        }
    }
}
