using CreateSiteMap.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateSiteMap.UI.Interfaces
{
    public interface IMainService
    {
        Task<PageResultView> ResultsAsync(string url);

        HistoryView GetHistory(string hostUrl);
    }
}
