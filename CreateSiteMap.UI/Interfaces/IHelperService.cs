using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateSiteMap.UI.Interfaces
{
    public interface IHelperService
    {
        Task<bool> IsUrlExistAsync(string url);

        string GetHostFromUrl(string url);
    }
}
