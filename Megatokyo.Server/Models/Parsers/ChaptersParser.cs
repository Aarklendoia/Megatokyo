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
        public static IEnumerable<Chapter> Parse(Uri url)
        {
            List<Chapter> chapters = new();
            HtmlWeb web = new();
            HtmlDocument htmlDoc = web.Load(url);
            HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//body/div[@id='typelinks']/div/ul/li/a");
            foreach (HtmlNode node in nodes)
            {
                StringExtractor stringExtractor = new(node.OuterHtml);
                StringExtractor categoryExtractor = new(node.GetAttributeValue("href", ""));
                int number = 0;
                string title;
                if (node.OuterHtml.Contains("Chapter", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!node.OuterHtml.Contains("Chapter 0", StringComparison.InvariantCultureIgnoreCase))
                    {
                        number = int.Parse(stringExtractor.Extract("Chapter ", ":", false), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        title = stringExtractor.Extract("&quot;", "&quot;", false);
                    }
                    else
                    {
                        title = "Prologue";
                    }
                }
                else
                {
                    title = stringExtractor.Extract(">", "<", false);
                }
                categoryExtractor.Remove("ar", "#", true, out string category);
                chapters.Add(new Chapter(number, title, category));
            }
            return chapters;
        }
    }
}
