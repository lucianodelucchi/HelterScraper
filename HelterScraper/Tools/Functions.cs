using HtmlAgilityPack;
using System;

namespace HelterScraper.Tools
{
   public class Functions
    {

        public static Func<string, HtmlDocument> parser =
            (htmlString) =>
            {
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlString);
                return htmlDocument;
            };

        public static Func<string, Uri> getBaseURL =
            (url) =>
            {
                var startUri = new Uri(url);
                var baseUri = startUri.Scheme + "://" + startUri.Authority;

                return new Uri(baseUri);
            };

        public static Func<string, string, Uri> buildFullURL =
            (absoluteUri, relativeUri) =>
            {
                var relative = new Uri(relativeUri, UriKind.Relative);

                return new Uri(getBaseURL(absoluteUri), relativeUri);
            };

        /// <summary>
        ///     <remarks>
        ///         Stolen from https://github.com/play/play-windows/blob/master/Play/ViewModels/WelcomeViewModel.cs#L101
        ///     </remarks>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Func<string, bool> isValidURL =
            (url) =>
            {
                if (String.IsNullOrWhiteSpace(url)) return false;

                try
                {
                    var dontcare = new Uri(url);
                }
                catch (Exception ex)
                {
                    return false;
                }

                return url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase);
            };
    }
}
