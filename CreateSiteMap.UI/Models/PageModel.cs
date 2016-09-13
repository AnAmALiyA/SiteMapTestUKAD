using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreateSiteMap.UI.Models
{
    public class PageModel
    {
        public string Url { get; set; }
        public IEnumerable<HistoryModel> History { get; set; }
    }
}