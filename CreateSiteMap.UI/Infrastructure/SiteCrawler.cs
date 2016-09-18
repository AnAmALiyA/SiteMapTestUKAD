using CreateSiteMap.UI.Models;
using CreateSiteMap.UI.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace CreateSiteMap.UI.Infrastructure
{
    public class SiteCrawler
    {
        public int MaxPageAllowed { get; set; }
        public SiteCrawler()
        {
            MaxPageAllowed = 100;
        }

        private string CorrectUrl(string url)
        {
            string lastSymbol = string.Empty;          

            if (url.Length>1)
            {
                    lastSymbol = url.Substring(url.Length - 2, 2); 
                              
                if (lastSymbol == "/#")
                {
                    url = url.Remove(url.Length - 2);
                    return url;
                }
            }            
           
                lastSymbol = url.Substring(url.Length - 1, 1);
                if (lastSymbol == "/" || lastSymbol == "#")
                {
                    url = url.Remove(url.Length - 1);
                    return url;
                }

            return url;
        }

        public async Task<IEnumerable<MesuaredPage>> CrawlAsync(string url)
        {
            ConcurrentDictionary<string, MesuaredPage> crawledPagesDictionary = new ConcurrentDictionary<string, MesuaredPage>();
            string host = new HelperService().GetHostFromUrl(url);

            await CrawlPage(crawledPagesDictionary, host, CorrectUrl(url));

            return crawledPagesDictionary.Select(x => x.Value).ToList();
        }
        
        private async Task CrawlPage(ConcurrentDictionary<string, MesuaredPage> crawledPagesDictionary, string host, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL can not be null");

            if (crawledPagesDictionary.ContainsKey(url) || crawledPagesDictionary.Count == MaxPageAllowed)
            {
                return;
            }

            List<string> pagesCrawledList = new List<string>();
            string resultHttpClient = "";
            Regex regexLink = new Regex("<a[^>]*? href=\"(?<url>[^\"]+)\"[^>]*?>(?<text>.*?)</a>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            MatchCollection matches;
            Stopwatch stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(url))
                using (HttpContent content = response.Content)
                {
                    stopwatch.Stop();

                    if (!response.IsSuccessStatusCode)
                    {
                        return;
                    }

                    resultHttpClient = await content.ReadAsStringAsync();
                    matches = regexLink.Matches(resultHttpClient);
                }
            }
            catch (HttpRequestException)
            {
                return;
            }

            bool isAdded = crawledPagesDictionary.TryAdd(url, new MesuaredPage { Url = url, ResponseTime = (int)stopwatch.ElapsedMilliseconds, Date = DateTime.Now });

            if (!isAdded || matches.Count == 0)
            {
                return;
            }

            string link = string.Empty;
            foreach (Match match in matches)
            {
                link = match.Groups[1].Value;
                if (string.IsNullOrWhiteSpace(link))
                {
                    continue;
                }

                if (!link.StartsWith("http") && !link.StartsWith("www") && !link.StartsWith(host))
                {
                    if (!link.StartsWith("/"))
                    {
                        link = "/" + link;
                    }
                    link = host + link;
                }

                if (link.StartsWith(host))
                {
                    link = CorrectUrl(link);

                    if (!crawledPagesDictionary.ContainsKey(link) && !pagesCrawledList.Contains(link))
                    {
                        pagesCrawledList.Add(link);
                    }
                }
            }

            if (pagesCrawledList.Count == 0)
            {
                return;
            }

            int length = pagesCrawledList.Count;
            if (pagesCrawledList.Count + crawledPagesDictionary.Count > MaxPageAllowed)
            {
                length = MaxPageAllowed - crawledPagesDictionary.Count;
            }

            for (int i = 0; i < length; i++)
            {
                await CrawlPage(crawledPagesDictionary, host, pagesCrawledList[i]);
            }
            return;
        }

    }
}
