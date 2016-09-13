using CreateSiteMap.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CreateSiteMap.DAL.Interfaces
{
    public interface IRepository : IDisposable
    {        
        Task<int> SaveChangesAsync();

        void AddHost(Host host);
        void UpdatePage(Page page);
        void UpdateHost(Host host);

        Host GetHostByUrl(string hostUrl);
        Host GetHostByUrlIncludPages(string hostUrl);
        IEnumerable<History>  GetHistoryByHost(string hostUrl);
    }
}
