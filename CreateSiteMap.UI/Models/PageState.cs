
namespace CreateSiteMap.UI.Models
{
    public class PageState
    {        
        public string Url { get; set; }

        public int MinResponseTime { get; set; }
        public int MaxResponseTime { get; set; }
        public int ResponseTime { get; set; }        
    }
}