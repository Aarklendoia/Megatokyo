using Megatokyo.Server.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Megatokyo.Server.Models
{
    public class Rant
    {
        public int RantId { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }
        public Uri Url { get; set; }
        public DateTime DateTime { get; set; }
        public string Content { get; set; }

        public Rant()
        {

        }

        public Rant(Rants rant)
        {
            if (rant != null)
            {
                Number = rant.Number;
                Author = rant.Author;
                Url = null;
                DateTime = rant.Date;
                Content = "";
            }
        }
    }
}
