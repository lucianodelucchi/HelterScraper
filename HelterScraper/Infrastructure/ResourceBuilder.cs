using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using Fizzler.Systems.HtmlAgilityPack;

namespace HelterScraper.Infrastructure
{
    public class ResourceBuilder
    {
        protected readonly Func<string, HtmlDocument> htmlParser;

        protected HtmlDocument htmlDocument;

        public ResourceBuilder(Func<string, HtmlDocument> parser)
        {
            htmlParser = parser;
        }

        public void ParseHtml(string html)
        {
            htmlDocument = htmlParser(html);
        }

        public HtmlNode GetNodeFromSelector(string selector)
        {
            return htmlDocument.DocumentNode.QuerySelector(selector);
        }

        public string GetTextFromSelector(string selector)
        {
            return GetNodeFromSelector(selector).InnerText;
        }

        public IEnumerable<HtmlNode> GetNodesFromSelector(string selector)
        {
            return htmlDocument.DocumentNode.QuerySelectorAll(selector);
        }

        public string GetAttribute(string selector, string attribute)
        {
            return GetNodeFromSelector(selector).GetAttributeValue(attribute, string.Empty);
        }

        public IEnumerable<Tuple<string, string>> ScrapeAttributeTuples(string selector, string attribute)
        {
            return GetNodesFromSelector(selector)
                .Select(x => new { Name = x.InnerText, Url = x.GetAttributeValue(attribute, string.Empty) })
                .Where(x => x.Url != string.Empty)
                .Select(x => new Tuple<string, string>(x.Name, x.Url));
        }

        /// <summary>
        /// For the love of the almighty come up with a better method please!
        /// </summary>
        /// <param name="collectionSelector"></param>
        /// <param name="tuple1Selector">
        ///     <remarks>This would be the resource id, it will select the href attribute so the selector should be a link.</remarks>
        ///     <example>eg:While scraping scorers from a match, this param should select the URL pointing to the player profile/bio.</example>
        /// </param>
        /// <param name="tuple2Selector"></param>
        /// <param name="tuple3Selector"></param>
        /// <returns></returns>
        //public IEnumerable<Tuple<string, IEnumerable<string>, IEnumerable<string>>> ScrapeTriplet(string collectionSelector, string tuple1Selector, string tuple2Selector, string tuple3Selector)
        //{
        //    var collection = GetNodesFromSelector(collectionSelector);
        //    var ret = collection.Select(x =>
        //        new Tuple<string, IEnumerable<string>, IEnumerable<string>>(
        //            String.Join(",", x.QuerySelectorAll(tuple1Selector).Select(y => y.GetAttributeValue("href", string.Empty))),
        //            x.QuerySelectorAll(tuple2Selector).Select(y => y.InnerText.Trim()),
        //            x.QuerySelectorAll(tuple3Selector).Select(y => y.InnerText.Replace(",", string.Empty).Trim())
        //    ));
        //    return ret;
        //}

        public IEnumerable<Tuple<string, string, IEnumerable<string>>> ScrapeTriplet(string collectionSelector, string tuple1Selector, string tuple2Selector, string tuple3Selector)
        {
            var collection = GetNodesFromSelector(collectionSelector);
            var ret = collection.Select(x =>
                new Tuple<string, string, IEnumerable<string>>(
                    String.Join(",", x.QuerySelectorAll(tuple1Selector).Select(y => y.GetAttributeValue("href", string.Empty))),
                    x.QuerySelector(tuple2Selector).InnerText.Trim(),
                    x.QuerySelectorAll(tuple3Selector).Select(y => y.InnerText.Replace(",", string.Empty).Trim())
            ));
            return ret;
        }
    }
}
