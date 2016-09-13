using CreateSiteMap.DAL.EF;
using CreateSiteMap.DAL.Entities;
using CreateSiteMap.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;

namespace CreateSiteMap.DAL.Repositories
{
    public class PageRepository: IRepository
    {
        private SiteMapContext _dbContext;
        private bool _disposed;

        public PageRepository()
        {
            _dbContext = new SiteMapContext();
        }
                
        public Task<int> SaveChangesAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
        
        public void AddHost(Host host)
        {
            _dbContext.Entry(host).State = EntityState.Added;
        }
        
        public void UpdatePage(Page page)
        {
            Page findePage = _dbContext.Pages.Find(page.Id);
            if (findePage != null)
            {
                _dbContext.Entry(page).State = EntityState.Modified;
            }
        }

        public void UpdateHost(Host host)
        {
            Host findedHost = _dbContext.Hosts.Find(host.Id);
            if (findedHost!=null)
            {
                _dbContext.Entry(host).State = EntityState.Modified;
            }
        }

        public Host GetHostByUrl(string hostUrl)
        {
            return _dbContext.Hosts.SingleOrDefault(x => x.HostName == hostUrl);
        }

        public Host GetHostByUrlIncludPages(string hostUrl)
        {
            return _dbContext.Hosts
                .Include(x => x.Pages)
                .SingleOrDefault(x => x.HostName == hostUrl);
        }
                
        public IEnumerable<History> GetHistoryByHost(string hostUrl)
        {
            return _dbContext.History
                .Include(x=>x.Page)
                .Include(y=>y.Page.Host)
                .Where(x=>x.Page.Host.HostName==hostUrl)
                .AsNoTracking()
                .ToList();
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _dbContext.Dispose();
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
