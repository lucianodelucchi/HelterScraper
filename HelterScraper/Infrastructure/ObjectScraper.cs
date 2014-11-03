using Akavache;
using HelterScraper.Tools;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using HelterScraper.Extensions;
using System.Reflection;

namespace HelterScraper.Infrastructure
{
    public interface ScraperService
    {
        string StartUrl { get; }

        bool Supports(string url);
    }

    public interface ScraperService<T> : ScraperService
    {
        IObservable<T> Scrape(string url);
    }

    public abstract class ObjectScraper<T> : ScraperService<T>
    {
        protected abstract string BaseURL { get; }

        string startUrl;

        public string StartUrl { get { return startUrl; } }

        protected readonly IBlobCache cache;

        protected readonly Func<ResourceBuilder> builderFactory;

        public ObjectScraper()
        {
            BlobCache.ApplicationName = Assembly.GetEntryAssembly().GetName().Name;
            cache = BlobCache.LocalMachine;
            
            Func < ResourceBuilder > resourceBuilder = () => new ResourceBuilder(Functions.parser);
            builderFactory = resourceBuilder;
        }

        protected string BuildAbsoluteUrl(string relativeUrl)
        {
            return Functions.buildFullURL(StartUrl, relativeUrl).ToString();
        }

        public IObservable<T> Scrape(string url)
        {
            startUrl = url;

            return cache.DownloadUrl(url)
                    .OnErrorResumeNext(Observable.Return(new byte[] { }))
                    .Where(x => x.Length != 0)
                    .Select(bytes => bytes.EncodeToString())
                    .Select(ScrapeToObject)
                    ;
        }

        protected abstract T ScrapeToObject(string html);

        public bool Supports(string url)
        {
            var isValid = Functions.isValidURL(url);

            if (!isValid)
            {
                return false;
            }

            var baseURL = new Uri(BaseURL);

            var compareToURL = new Uri(url);

            var baseSegments = new HashSet<string>(baseURL.Segments);
            var compareToSegments = new HashSet<string>(compareToURL.Segments);

            return baseURL.Host == compareToURL.Host && baseSegments.IsSubsetOf(compareToSegments);
        }
    }
}
