using System.Collections.Generic;

namespace CreateSiteMap.UI.Models
{
    public class HistoryView
    {
        public string HostUrl { get; set; }
        public ICollection<PageModel> Pages { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }

        public static HistoryView CreateHistoryError(string hostUrl, string errorMessage)
        {
            return new HistoryView { Success = false, HostUrl = hostUrl, Error = errorMessage };
        }
    }
}