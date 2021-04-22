﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Megatokyo.Server.Models.Syndication
{
    /// <summary>
    /// Un simple analyseur de flux RSS, RDF et ATOM.
    /// </summary>
    internal class FeedParser
    {
        /// <summary>
        /// Analyse le <see cref="FeedType"/> donné et retourne un <see cref="IList&lt;Item&gt;"/>.
        /// </summary>
        /// <returns></returns>
        public IList<Item> Parse(Uri url, FeedType feedType)
        {
            return feedType switch
            {
                FeedType.RSS => ParseRss(url),
                FeedType.RDF => ParseRdf(url),
                FeedType.Atom => ParseAtom(url),
                _ => throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "{0} is not supported", feedType.ToString())),
            };
        }

        /// <summary>
        /// Analyse un flux Atom et retourne un <see cref="IList&lt;Item&gt;"/>.
        /// </summary>
        public virtual IList<Item> ParseAtom(Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            try
            {
                XDocument doc = XDocument.Load(url.OriginalString);
                // Feed/Entry
                IEnumerable<Item> entries = from item in doc.Root.Elements().Where(i => i.Name.LocalName == "entry")
                                            select new Item
                                            {
                                                FeedType = FeedType.Atom,
                                                Content = item.Elements().First(i => i.Name.LocalName == "content").Value,
                                                Link = item.Elements().First(i => i.Name.LocalName == "link").Attribute("href").Value,
                                                PublishDate = ParseDate(item.Elements().First(i => i.Name.LocalName == "published").Value),
                                                Title = item.Elements().First(i => i.Name.LocalName == "title").Value
                                            };
                return entries.ToList();
            }
            catch (UriFormatException e)
            {
                Debug.WriteLine(e.Message);
                return new List<Item>();
            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine(e.Message);
                return new List<Item>();
            }
        }

        /// <summary>
        /// Analyse un flux RSS et retourne un <see cref="IList&lt;Item&gt;"/>.
        /// </summary>
        public virtual IList<Item> ParseRss(Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            try
            {
                XDocument doc = XDocument.Load(url.OriginalString);
                // RSS/Channel/item
                IEnumerable<Item> entries = from item in doc.Root.Descendants().First(i => i.Name.LocalName == "channel").Elements().Where(i => i.Name.LocalName == "item")
                                            select new Item
                                            {
                                                FeedType = FeedType.RSS,
                                                Content = item.Elements().First(i => i.Name.LocalName == "description").Value,
                                                Link = item.Elements().First(i => i.Name.LocalName == "link").Value,
                                                PublishDate = ParseDate(item.Elements().First(i => i.Name.LocalName == "pubDate").Value),
                                                Title = item.Elements().First(i => i.Name.LocalName == "title").Value
                                            };
                return entries.ToList();
            }
            catch (UriFormatException e)
            {
                Debug.WriteLine(e.Message);
                return new List<Item>();
            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine(e.Message);
                return new List<Item>();
            }
        }

        /// <summary>
        /// Analyse un flux RDF et retourne un <see cref="IList&lt;Item&gt;"/>.
        /// </summary>
        public virtual IList<Item> ParseRdf(Uri url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            try
            {
                XDocument doc = XDocument.Load(url.OriginalString);
                // <item> is under the root
                IEnumerable<Item> entries = from item in doc.Root.Descendants().Where(i => i.Name.LocalName == "item")
                                            select new Item
                                            {
                                                FeedType = FeedType.RDF,
                                                Content = item.Elements().First(i => i.Name.LocalName == "description").Value,
                                                Link = item.Elements().First(i => i.Name.LocalName == "link").Value,
                                                PublishDate = ParseDate(item.Elements().First(i => i.Name.LocalName == "date").Value),
                                                Title = item.Elements().First(i => i.Name.LocalName == "title").Value
                                            };
                return entries.ToList();
            }
            catch (UriFormatException e)
            {
                Debug.WriteLine(e.Message);
                return new List<Item>();
            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine(e.Message);
                return new List<Item>();
            }
        }

        private static DateTime ParseDate(string date)
        {
            if (DateTime.TryParse(date, out DateTime result))
            {
                return result;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
    }
    /// <summary>
    /// Représente le format XML d'un flux.
    /// </summary>
    public enum FeedType
    {
        /// <summary>
        /// Format Really Simple Syndication.
        /// </summary>
        RSS,
        /// <summary>
        /// Format de résumé de site RDF.
        /// </summary>
        RDF,
        /// <summary>
        /// Format de syndication Atom.
        /// </summary>
        Atom
    }
}
