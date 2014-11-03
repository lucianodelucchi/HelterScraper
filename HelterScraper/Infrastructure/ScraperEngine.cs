using System;
using System.Collections.Generic;
using System.Linq;

namespace HelterScraper.Infrastructure
{
    public class ScraperEngine
    {
        readonly List<Func<ScraperService>> scraperServices = new List<Func<ScraperService>>();

        public void RegisterService(Func<ScraperService> service)
        {
            scraperServices.Add(service);
        }

        public Func<ScraperService<T>> Resolve<T>(string url)
        {
            var service = scraperServices.First(x => x().Supports(url));

            if (service == null)
            {
                throw new NotSupportedException(string.Format("The pattern {0} is not supported.", url));
            }

            return (Func<ScraperService<T>>)service;
        }
    }
}
