using HtmlAgilityPack;
using Megatokyo.Domain;
using Megatokyo.Models;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace Megatokyo.Server.Models.Parsers
{
    internal class StripsParser
    {
        private enum FileFormat { Png, Jpeg, Gif };

        public static async Task<List<Strip>> ParseAsync(Uri url, IEnumerable<Chapter> chapters, IEnumerable<Strip> stripsInDatabase)
        {
            List<Strip> strips = new();
            HtmlWeb web = new();
            HtmlDocument htmlDoc = web.Load(url);

            foreach (Chapter chapter in chapters)
            {
                IEnumerable<HtmlNode> nodes = htmlDoc.DocumentNode.SelectNodes("//li/a").Where(w => w.ParentNode.ParentNode.ParentNode.ParentNode.SelectSingleNode(".//a").Id == chapter.Category);
                foreach (HtmlNode node in nodes)
                {
                    Strip? strip = await ExtractStripAsync(node, chapter, stripsInDatabase);
                    if (strip != null)
                    {
                        strips.Add(strip);
                    }
                }
            }
            return strips;
        }

        private static async Task<Strip?> ExtractStripAsync(HtmlNode node, Chapter chapter, IEnumerable<Strip> stripsInDatabase)
        {
            StringExtractor stringExtractor = new(node.OuterHtml);
            DateTimeOffset publishDate = DateTime.ParseExact(stringExtractor.Extract("title=\"", "\" name=\"", false).Replace("th,", "", StringComparison.InvariantCultureIgnoreCase).Replace("rd,", "", StringComparison.InvariantCultureIgnoreCase).Replace("nd,", "", StringComparison.InvariantCultureIgnoreCase).Replace("st,", "", StringComparison.InvariantCultureIgnoreCase), "MMMM d yyyy", new CultureInfo("en-US"));
            int number = int.Parse(node.Attributes["name"].Value, CultureInfo.InvariantCulture);
            string title = WebUtility.HtmlDecode(stringExtractor.Extract(" - ", "<", false));
            Uri url = new("https://megatokyo.com/strips/" + number.ToString("D4", CultureInfo.InvariantCulture));
            if (!stripsInDatabase.Where(s => s.Number == number).Any())
            {
                Strip strip = new(chapter.Category, number, title, url, publishDate);
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

        private static async Task<bool> GetFileTypeAsync(Strip strip)
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
            HttpClient httpClient = new();
            try
            {
                HttpRequestMessage message = new(HttpMethod.Head, filePath);
                HttpResponseMessage response = await httpClient.SendAsync(message);
                bool result = response.StatusCode == HttpStatusCode.OK;
                if (result)
                    Debug.WriteLine("File found : " + filePath);
                else
                    Debug.WriteLine("ERROR : file not found " + filePath);
                return result;
            }
            catch (WebException e)
            {
                Debug.WriteLine("ERROR : file not found " + filePath + " (" + e.Message + ")");
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
