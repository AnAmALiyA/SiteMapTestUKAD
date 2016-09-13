using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CreateSiteMap.UI.Models
{
    public class MesuaredPage
    {
        public string Url { get; set; }
        public DateTime Date { get; set; }
        public int ResponseTime { get; set; }
    }
}