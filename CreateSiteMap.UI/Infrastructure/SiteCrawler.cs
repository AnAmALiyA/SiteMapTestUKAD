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
            string lastSymbol = url.Substring(url.Length - 1, 1);
            if (lastSymbol == "/" || lastSymbol == "#") //delete '/' or '#' from the end of url if it exist
                url = url.Remove(url.Length - 1);

            return url;
        }

        public async Task<IEnumerable<MesuaredPage>> CrawlAsync(string url)
        {
            //Представляет потокобезопасную коллекцию пар "ключ-значение", доступ к которой могут одновременно получать несколько потоков.
            ConcurrentDictionary<string, MesuaredPage> crawledPagesDictionary = new ConcurrentDictionary<string, MesuaredPage>();
            string host = new HelperService().GetHostFromUrl(url);

            await CrawlPage(crawledPagesDictionary, host, CorrectUrl(url));

            return crawledPagesDictionary.Select(x => x.Value).ToList();
        }

        //Хитрость этого метода в том, что он создаёт временный List. В него сохраняет найденное и проверяет перед сохранением в базовый Dictionaty
        private async Task CrawlPage(ConcurrentDictionary<string, MesuaredPage> crawledPagesDictionary, string host, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL can not be null");

            if (crawledPagesDictionary.ContainsKey(url) || crawledPagesDictionary.Count == MaxPageAllowed)
            {
                return;
            }

            List<string> pagesCrawledList = new List<string>(); //list of parsed links
            string resultHttpClient = ""; //result of HttpClient
            Regex regexLink = new Regex(@"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>"); //reg to find links on html page
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

                    //if http status not 200 - stop executing method Если удачно, то false и я не зайду в if
                    if (!response.IsSuccessStatusCode)
                    {
                        return;
                    }

                    resultHttpClient = await content.ReadAsStringAsync();
                    matches = regexLink.Matches(resultHttpClient);
                }
            }
            catch (HttpRequestException) //raises when has been pased nonexistent url -> angular ui url or something else
            {
                return; //nothing to do when url not exist
            }

            bool isAdded = crawledPagesDictionary.TryAdd(url, new MesuaredPage { Url = url, ResponseTime = (int)stopwatch.ElapsedMilliseconds, Date = DateTime.Now });

            if (!isAdded || matches.Count == 0) //if page already crawled or hasn't links
            {
                return;
            }

            //get parsed links
            string link = string.Empty;
            foreach (Match match in matches)
            {
                link = match.Groups[1].Value;
                if (string.IsNullOrWhiteSpace(link))
                {
                    continue;
                }

                if (!link.StartsWith("http") && !link.StartsWith("www") && !link.StartsWith(host)) //transform absolute path to absolure url
                {
                    link = host + "/" + link;
                }

                if (link.StartsWith(host)) //get only internal links
                {
                    link = CorrectUrl(link);

                    if (!crawledPagesDictionary.ContainsKey(link) && !pagesCrawledList.Contains(link))
                    {
                        pagesCrawledList.Add(link);
                    }
                }
            }

            if (pagesCrawledList.Count == 0)//if html page hasn't internal links
            {
                return;
            }

            //check MaxPageToCrawling. Тут условия длины проверки.
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