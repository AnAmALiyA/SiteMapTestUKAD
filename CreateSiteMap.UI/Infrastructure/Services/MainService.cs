using CreateSiteMap.DAL.Interfaces;
using CreateSiteMap.DAL.Repositories;
using CreateSiteMap.UI.Infrastructure;
using CreateSiteMap.UI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreateSiteMap.DAL.Entities;
using System.Linq;
using System;

namespace CreateSiteMap.UI.Services
{
    public class MainService
    {
        private SiteCrawler _siteCrawler;
        private IRepository _repository;
        private HelperService _helperService;
        private bool _disposed;

        public MainService()
        {
            _siteCrawler = new SiteCrawler();
            _repository = new PageRepository();
            _helperService = new HelperService();
        }

        public async Task<PageResultView> ResultsAsync(string url)
        {
            bool isUrlExist = await _helperService.IsUrlExistAsync(url);
            if (isUrlExist)
            {
                IEnumerable<MesuaredPage> mesuaredPages = await _siteCrawler.CrawlAsync(url);
                IEnumerable<PageState> pagesState = await SaveResultsAsync(mesuaredPages);

                return GetPageStateView(pagesState);
            }

            return PageResultView.CreateResultWithError(url);
        }

        private async Task<IEnumerable<PageState>> SaveResultsAsync(IEnumerable<MesuaredPage> mesuaredPages)
        {
            if (mesuaredPages.Count()==0)
            {
                return null;
            }

            bool isUrlExistInDb = IsUrlExistInDb(mesuaredPages.First().Url);
            if (isUrlExistInDb)
            {
                return await UpdateHistoryAsync(mesuaredPages);
            }
            else
            {
                return await AddHostAsync(mesuaredPages);
            }
        }

        private bool IsUrlExistInDb(string url)
        {
            string hostUrl = _helperService.GetHostFromUrl(url);
            Host findedHostUrl = _repository.GetHostByUrl(hostUrl);
            return findedHostUrl != null;
        }

        private async Task<IEnumerable<PageState>> UpdateHistoryAsync(IEnumerable<MesuaredPage> mesuaredPages)
        {            
            List<PageState> resultPage = new List<PageState>(mesuaredPages.Count());

            string hostUrl = _helperService.GetHostFromUrl(mesuaredPages.First().Url);
            Host host = _repository.GetHostByUrlIncludPages(hostUrl);

            foreach (MesuaredPage page in mesuaredPages)
            {
                Page pageUpdate = host.Pages.FirstOrDefault(x => x.Url == page.Url);              
                if (pageUpdate!=null)
                {   
                    if (pageUpdate.MinResponseTime > page.ResponseTime)
                    {
                        pageUpdate.MinResponseTime = page.ResponseTime;
                    }
                    if(pageUpdate.MaxResponseTime<page.ResponseTime)
                    {
                        pageUpdate.MaxResponseTime = page.ResponseTime;
                    }

                    pageUpdate.History.Add(new History
                    {
                        Date = page.Date,
                        ResponseTime = page.ResponseTime
                    });

                    resultPage.Add(new PageState
                    {
                        Url = page.Url,
                        MinResponseTime = pageUpdate.MinResponseTime,
                        MaxResponseTime = pageUpdate.MaxResponseTime,
                        ResponseTime = page.ResponseTime
                    });

                    _repository.UpdatePage(pageUpdate);
                }
                else 
                {
                    Page pageNew = new Page
                    {
                        Url = page.Url,
                        MinResponseTime = page.ResponseTime,
                        MaxResponseTime = page.ResponseTime
                    };

                    pageNew.History.Add(new History
                    {
                        Date = page.Date,
                        ResponseTime = page.ResponseTime
                    });

                    resultPage.Add(new PageState
                    {
                        Url = page.Url,
                        MinResponseTime = page.ResponseTime,
                        MaxResponseTime = page.ResponseTime,
                        ResponseTime = page.ResponseTime
                    });

                    host.Pages.Add(pageNew);

                    _repository.UpdateHost(host);
                }
            }

            await _repository.SaveChangesAsync();
            return resultPage;            
        }

        private async Task<IEnumerable<PageState>> AddHostAsync(IEnumerable<MesuaredPage> mesuaredPages)
        {  
            List<PageState> resultPage = new List<PageState>(mesuaredPages.Count());

            Host host = new Host
            {
                HostName = _helperService.GetHostFromUrl(mesuaredPages.First().Url)
            };

            foreach (MesuaredPage page in mesuaredPages)
            {
                Page pageInHost = new Page
                {
                    Url = page.Url,
                    MinResponseTime = page.ResponseTime,
                    MaxResponseTime = page.ResponseTime
                };

                resultPage.Add(new PageState
                {
                    Url = page.Url,
                    MinResponseTime = page.ResponseTime,
                    MaxResponseTime = page.ResponseTime,
                    ResponseTime = page.ResponseTime
                });

                pageInHost.History.Add(new History
                {
                    Date = page.Date,
                    ResponseTime = page.ResponseTime
                });

                host.Pages.Add(pageInHost);
            }

            _repository.AddHost(host);
            await _repository.SaveChangesAsync();

            return resultPage;
        }

        private PageResultView GetPageStateView(IEnumerable<PageState> pagesState)
        {
            PageResultView resultView = new PageResultView();

            if (pagesState!=null)
            {       
                resultView.Pages = pagesState.OrderByDescending(x => x.ResponseTime).ToList();              
                resultView.AverageResponseTime = pagesState.Average(x => x.ResponseTime);
            }

            resultView.Success = resultView.Pages != null;
            
            resultView.HostUrl = resultView.Success ? 
                _helperService.GetHostFromUrl(pagesState.First().Url) : "";

            return resultView;
        }

        public HistoryView GetHistory(string hostUrl)
        {
            if (string.IsNullOrWhiteSpace(hostUrl))
            {
                return HistoryView.CreateHistoryError(hostUrl, "Please enter url");
            }

            var gruped = _repository.GetHistoryByHost(hostUrl).GroupBy(x => x.Page.Url).ToList();

            if (gruped.Count==0)
            {
                return HistoryView.CreateHistoryError(hostUrl, "History is empty");
            }

            HistoryView history = new HistoryView
            {
                HostUrl = hostUrl,
                Success = true,
                Pages = new List<PageModel>()
            };

            foreach (var page in gruped)
            {
                PageModel pageNew = new PageModel();
                pageNew.Url = page.Key;
                pageNew.History = page.OrderByDescending(x => x.Date).Select(x => new HistoryModel
                {
                    Date = x.Date.ToString("MM/dd/yyyy HH:mm:ss"),
                    ResponseTime = x.ResponseTime
                }).ToList();

                history.Pages.Add(pageNew);
            }

            return history;
        }
        
        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _repository.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}