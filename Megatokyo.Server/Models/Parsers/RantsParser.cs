using HtmlAgilityPack;
using Megatokyo.Domain;
using Megatokyo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;

namespace Megatokyo.Server.Models.Parsers
{
    internal static class RantsParser
    {
        public static List<RantDomain> Parse(int stripNumberMax)
        {
            return Parse(1, stripNumberMax);
        }

        public static List<RantDomain> Parse(int stripNumber, int stripNumberMax)
        {
            List<RantDomain> rants = new();
            HtmlWeb web = new();
            for (int index = stripNumber; index <= stripNumberMax; index++)
            {
                HtmlDocument htmlDoc = web.Load(new Uri("https://megatokyo.com/strip/" + index.ToString(CultureInfo.InvariantCulture)));
                HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@id,'rant')]");
                foreach (HtmlNode node in nodes)
                {
                    RantDomain rant = ExtractRant(node, rants);
                    if (rant != null)
                    {
                        rants.Add(rant);
                        Debug.WriteLine("Rant " + rant.Number.ToString(CultureInfo.InvariantCulture) + " - " + rant.Title);
                    }
                }
            }
            return rants;
        }

        private static RantDomain ExtractRant(HtmlNode node, List<RantDomain> rants)
        {
            StringExtractor stringExtractor = new(node.Attributes["id"].Value);
            stringExtractor.Remove("r", "t", true, out string extractednumber);
            int number = int.Parse(extractednumber, CultureInfo.InvariantCulture);
            if (rants.Where(r => r.Number == number).Any())
            {
                return null;
            }
            string author = node.SelectSingleNode(".//h3").InnerHtml.Replace("&gt;", "", StringComparison.InvariantCultureIgnoreCase).Replace("&lt;", "", StringComparison.InvariantCultureIgnoreCase).Trim();
            StringExtractor titleExtractor = new(WebUtility.HtmlDecode(node.SelectSingleNode(".//h4/a").InnerHtml));
            string title = titleExtractor.Extract("\"", "\"", false).Trim();
            DateTimeOffset publishDate = DateTime.ParseExact(node.SelectSingleNode(".//p[contains(@class,'date')]").InnerHtml, "dddd - MMMM d, yyyy", new CultureInfo("en-US"));
            Uri url = new("https://megatokyo.com/" + node.SelectSingleNode(".//img").Attributes["src"].Value);
            string content = node.SelectSingleNode(".//div[contains(@class,'rantbody')]").InnerHtml;
            return new RantDomain(title, number, author, url, publishDate, content);
        }
    }
}