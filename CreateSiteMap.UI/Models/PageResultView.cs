using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreateSiteMap.UI.Models
{  
    public class PageResultView
    {
        public IEnumerable<PageState> Pages { get; set; }
        public bool Success { get; set; }
        public string HostUrl { get; set; }
        public string Error { get; set; }
        public double AverageResponseTime { get; set; }

        public static PageResultView CreateResultWithError(string hostUrl)
        {
            return new PageResultView { Success = false, Error = string.Format("Unable to find a {0}. Are you sure that url is correct?", hostUrl), HostUrl = hostUrl };          
        }

        //Not hard code
        //public static PageResultView CreateResultWithError(string messageError, string hostUrl)
        //{
        //    return new PageResultView { Success = false, Error = messageError, HostUrl = hostUrl };
        //}
    }
}