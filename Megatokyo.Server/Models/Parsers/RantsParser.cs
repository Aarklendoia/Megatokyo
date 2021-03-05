using HtmlAgilityPack;
using Megatokyo.Models;
using Megatokyo.Server.Models.Entities;
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
        public static List<Rant> Parse(int stripNumberMax)
        {
            return Parse(1, stripNumberMax);
        }

        public static List<Rant> Parse(int stripNumber, int stripNumberMax)
        {
            List<Rant> rants = new List<Rant>();
            HtmlWeb web = new HtmlWeb();
            for (int index = stripNumber; index <= stripNumberMax; index++)
            {
                HtmlDocument htmlDoc = web.Load(new Uri("https://megatokyo.com/strip/" + index.ToString(CultureInfo.InvariantCulture)));
                HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@id,'rant')]");
                foreach (HtmlNode node in nodes)
                {
                    Rant rant = ExtractRant(node, rants);
                    if (rant != null)
                    {
                        rants.Add(rant);
                        Debug.WriteLine("Rant " + rant.Number.ToString(CultureInfo.InvariantCulture) + " - " + rant.Title);
                    }
                }
            }
            return rants;
        }

        private static Rant ExtractRant(HtmlNode node, List<Rant> rants)
        {
            Rant rant = new Rant();
            StringExtractor stringExtractor = new StringExtractor(node.Attributes["id"].Value);
            stringExtractor.Remove("r", "t", true, out string number);
            rant.Number = int.Parse(number, CultureInfo.InvariantCulture);
            if (rants.Where(r => r.Number == rant.Number).Any())
            {
                return null;
            }

            rant.Author = node.SelectSingleNode(".//h3").InnerHtml.Replace("&gt;", "", StringComparison.InvariantCultureIgnoreCase).Replace("&lt;", "", StringComparison.InvariantCultureIgnoreCase).Trim();
            StringExtractor titleExtractor = new StringExtractor(WebUtility.HtmlDecode(node.SelectSingleNode(".//h4/a").InnerHtml));
            rant.Title = titleExtractor.Extract("\"", "\"", false).Trim();
            rant.DateTime = DateTime.ParseExact(node.SelectSingleNode(".//p[contains(@class,'date')]").InnerHtml, "dddd - MMMM d, yyyy", new CultureInfo("en-US"));
            rant.Url = new Uri("https://megatokyo.com/" + node.SelectSingleNode(".//img").Attributes["src"].Value);
            rant.Content = node.SelectSingleNode(".//div[contains(@class,'rantbody')]").InnerHtml;
            return rant;
        }
    }
}