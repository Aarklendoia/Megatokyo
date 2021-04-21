using HtmlAgilityPack;
using Megatokyo.Domain;
using Megatokyo.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Megatokyo.Server.Models.Parsers
{
    internal class ChaptersParser
    {
        public static IList<ChapterDomain> Parse(Uri url)
        {
            IList<ChapterDomain> chapters = new List<ChapterDomain>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(url);
            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//body/div[@id='typelinks']/div/ul/li/a");
            foreach (HtmlNode node in nodes)
            {
                StringExtractor stringExtractor = new StringExtractor(node.OuterHtml);
                StringExtractor categoryExtractor = new StringExtractor(node.GetAttributeValue("href", ""));
                ChapterDomain chapter = new();
                if (node.OuterHtml.Contains("Chapter", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!node.OuterHtml.Contains("Chapter 0", StringComparison.InvariantCultureIgnoreCase))
                    {
                        chapter.Number = int.Parse(stringExtractor.Extract("Chapter ", ":", false), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        chapter.Title = stringExtractor.Extract("&quot;", "&quot;", false);
                    }
                    else
                    {
                        chapter.Title = "Prologue";
                    }
                }
                else
                {
                    chapter.Title = stringExtractor.Extract(">", "<", false);
                }

                categoryExtractor.Remove("ar", "#", true, out string category);
                chapter.Category = category;
                chapters.Add(chapter);
            }
            return chapters;
        }
    }
}
