using HtmlAgilityPack;
using Megatokyo.Domain;
using Megatokyo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Megatokyo.Server.Models.Parsers
{
    internal class StripsParser
    {
        private enum FileFormat { Png, Jpeg, Gif };

        public static async Task<List<StripDomain>> ParseAsync(Uri url, IList<ChapterDomain> chapters, IEnumerable<StripDomain> stripsInDatabase)
        {
            List<StripDomain> strips = new List<StripDomain>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(url);

            foreach (ChapterDomain chapter in chapters)
            {
                IEnumerable<HtmlNode> nodes = htmlDoc.DocumentNode.SelectNodes("//li/a").Where(w => w.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode(".//a").Id == chapter.Category);
                foreach (HtmlNode node in nodes)
                {
                    StripDomain strip = await ExtractStripAsync(node, chapter.Category, stripsInDatabase);
                    if (strip != null)
                    {
                        strips.Add(strip);
                    }
                }
            }
            return strips;
        }

        private static async Task<StripDomain> ExtractStripAsync(HtmlNode node, string category, IEnumerable<StripDomain> stripsInDatabase)
        {
            StripDomain strip = new StripDomain();
            StringExtractor stringExtractor = new StringExtractor(node.OuterHtml);
            strip.Date = DateTime.ParseExact(stringExtractor.Extract("title=\"", "\" name=\"", false).Replace("th,", "", StringComparison.InvariantCultureIgnoreCase).Replace("rd,", "", StringComparison.InvariantCultureIgnoreCase).Replace("nd,", "", StringComparison.InvariantCultureIgnoreCase).Replace("st,", "", StringComparison.InvariantCultureIgnoreCase), "MMMM d yyyy", new CultureInfo("en-US"));
            strip.Number = int.Parse(node.Attributes["name"].Value, CultureInfo.InvariantCulture);
            strip.Title = WebUtility.HtmlDecode(stringExtractor.Extract(" - ", "<", false));
            strip.Url = new Uri("https://megatokyo.com/strips/" + strip.Number.ToString("D4", CultureInfo.InvariantCulture));
            strip.Category = category;
            if (!stripsInDatabase.Where(s => s.Number == strip.Number).Any())
            {
                if (await GetFileTypeAsync(strip))
                {
                    return strip;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private static async Task<bool> GetFileTypeAsync(StripDomain strip)
        {
            Uri filePath;
            bool formatFound;
            if (strip.Number < 1081)
            {
                filePath = GetImagePath(strip, FileFormat.Gif);
                formatFound = await CheckFileFormatAsync(filePath);
            }
            else
            {
                filePath = GetImagePath(strip, FileFormat.Png);
                formatFound = await CheckFileFormatAsync(filePath);
            }
            if (!formatFound)
            {
                filePath = GetImagePath(strip, FileFormat.Jpeg);
                formatFound = await CheckFileFormatAsync(filePath);
            }
            if (!formatFound)
            {
                if (strip.Number >= 1081)
                {
                    filePath = GetImagePath(strip, FileFormat.Gif);
                    formatFound = await CheckFileFormatAsync(filePath);
                }
                else
                {
                    filePath = GetImagePath(strip, FileFormat.Png);
                    formatFound = await CheckFileFormatAsync(filePath);
                }
            }
            strip.Url = filePath;
            return formatFound;
        }

        private static async Task<bool> CheckFileFormatAsync(Uri filePath)
        {
            WebRequest webRequest = WebRequest.Create(filePath);
            webRequest.Method = "HEAD";
            try
            {
                WebResponse response = await webRequest.GetResponseAsync();
                response.Dispose();
                Debug.WriteLine("File found : " + filePath);
                return true;
            }
            catch (WebException e)
            {
                Debug.WriteLine("ERROR : file note found " + filePath + " (" + e.Message + ")");
                return false;
            }
        }

        private static Uri GetImagePath(StripDomain strip, FileFormat fileType)
        {
            var filePath = fileType switch
            {
                FileFormat.Png => strip.Url + ".png",
                FileFormat.Gif => strip.Url + ".gif",
                _ => strip.Url + ".jpg",
            };
            return new Uri(filePath);
        }
    }
}
