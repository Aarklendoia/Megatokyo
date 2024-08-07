﻿using HtmlAgilityPack;
using Megatokyo.Domain;
using Megatokyo.Models;
using System.Diagnostics;
using System.Globalization;
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
            List<Rant> rants = [];
            HtmlWeb web = new();
            for (int index = stripNumber; index <= stripNumberMax; index++)
            {
                HtmlDocument htmlDoc = web.Load(new Uri("https://megatokyo.com/strip/" + index.ToString(CultureInfo.InvariantCulture)));
                HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@id,'rant')]");
                foreach (HtmlNode node in nodes)
                {
                    Rant? rant = ExtractRant(node, rants);
                    if (rant != null)
                    {
                        rants.Add(rant);
                        Debug.WriteLine("Rant " + rant.Number.ToString(CultureInfo.InvariantCulture) + " - " + rant.Title);
                    }
                }
            }
            return rants;
        }

        private static Rant? ExtractRant(HtmlNode node, List<Rant> rants)
        {
            StringExtractor stringExtractor = new(node.Attributes["id"].Value);
            stringExtractor.Remove("r", "t", true, out string extractednumber);
            int number = int.Parse(extractednumber, CultureInfo.InvariantCulture);
            if (rants.Exists(r => r.Number == number))
            {
                return null;
            }
            string author = node.SelectSingleNode(".//h3").InnerHtml.Replace("&gt;", "", StringComparison.InvariantCultureIgnoreCase).Replace("&lt;", "", StringComparison.InvariantCultureIgnoreCase).Trim();
            StringExtractor titleExtractor = new(WebUtility.HtmlDecode(node.SelectSingleNode(".//h4/a").InnerHtml));
            string title = titleExtractor.Extract("\"", "\"", false).Trim();
            DateTimeOffset publishDate = DateTime.ParseExact(node.SelectSingleNode(".//p[contains(@class,'date')]").InnerHtml, "dddd - MMMM d, yyyy", new CultureInfo("en-US"));
            Uri url = new("https://megatokyo.com/" + node.SelectSingleNode(".//img").Attributes["src"].Value);
            string content = node.SelectSingleNode(".//div[contains(@class,'rantbody')]").InnerHtml;
            return new Rant()
            {
                Number = number,
                Author = author,
                Title = title,
                PublishDate = publishDate,
                Url = url,
                Content = content
            };
        }
    }
}