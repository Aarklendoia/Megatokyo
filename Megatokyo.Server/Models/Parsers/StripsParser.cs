using HtmlAgilityPack;
using Megatokyo.Models;
using Megatokyo.Server.Database.Models;
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

        public static async Task<List<Strip>> ParseAsync(Uri url, IList<Chapter> chapters, IEnumerable<Strips> stripsInDatabase)
        {
            List<Strip> strips = new List<Strip>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDoc = web.Load(url);

            foreach (Chapter chapter in chapters)
            {
                IEnumerable<HtmlNode> nodes = htmlDoc.DocumentNode.SelectNodes("//li/a").Where(w => w.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode(".//a").Id == chapter.Category);
                foreach (HtmlNode node in nodes)
                {
                    Strip strip = await ExtractStripAsync(node, chapter.Category, stripsInDatabase).ConfigureAwait(false);
                    if (strip != null)
                    {
                        strips.Add(strip);
                    }
                }
            }
            return strips;
        }

        private static async Task<Strip> ExtractStripAsync(HtmlNode node, string category, IEnumerable<Strips> stripsInDatabase)
        {
            Strip strip = new Strip();
            StringExtractor stringExtractor = new StringExtractor(node.OuterHtml);
            strip.Date = DateTime.ParseExact(stringExtractor.Extract("title=\"", "\" name=\"", false).Replace("th,", "", StringComparison.InvariantCultureIgnoreCase).Replace("rd,", "", StringComparison.InvariantCultureIgnoreCase).Replace("nd,", "", StringComparison.InvariantCultureIgnoreCase).Replace("st,", "", StringComparison.InvariantCultureIgnoreCase), "MMMM d yyyy", new CultureInfo("en-US"));
            strip.Number = int.Parse(node.Attributes["name"].Value, CultureInfo.InvariantCulture);
            strip.Title = WebUtility.HtmlDecode(stringExtractor.Extract(" - ", "<", false));
            strip.Url = new Uri("https://megatokyo.com/strips/" + strip.Number.ToString("D4", CultureInfo.InvariantCulture));
            strip.Category = category;
            if (!stripsInDatabase.Where(s => s.Number == strip.Number).Any())
            {
                if (await GetFileTypeAsync(strip).ConfigureAwait(false))
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

        private static async Task<bool> GetFileTypeAsync(Strip strip)
        {
            Uri filePath;
            bool formatFound;
            if (strip.Number < 1081)
            {
                filePath = GetImagePath(strip, FileFormat.Gif);
                formatFound = await CheckFileFormatAsync(filePath).ConfigureAwait(false);
            }
            else
            {
                filePath = GetImagePath(strip, FileFormat.Png);
                formatFound = await CheckFileFormatAsync(filePath).ConfigureAwait(false);
            }
            if (!formatFound)
            {
                filePath = GetImagePath(strip, FileFormat.Jpeg);
                formatFound = await CheckFileFormatAsync(filePath).ConfigureAwait(false);
            }
            if (!formatFound)
            {
                if (strip.Number >= 1081)
                {
                    filePath = GetImagePath(strip, FileFormat.Gif);
                    formatFound = await CheckFileFormatAsync(filePath).ConfigureAwait(false);
                }
                else
                {
                    filePath = GetImagePath(strip, FileFormat.Png);
                    formatFound = await CheckFileFormatAsync(filePath).ConfigureAwait(false);
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
                WebResponse response = await webRequest.GetResponseAsync().ConfigureAwait(false);
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

        private static Uri GetImagePath(Strip strip, FileFormat fileType)
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
