using System;
using System.Collections.Generic;

namespace CreateSiteMap.DAL.Entities
{
    public class Page
    {
        public Page()
        {
            this.History = new List<History>();
        }

        public int Id { get; set; }
        public int HostId { get; set; }
        public string Url { get; set; }

        public int MinResponseTime { get; set; }
        public int MaxResponseTime { get; set; }

        public virtual ICollection<History> History { get; set; }
        public virtual Host Host { get; set; }
    }
}
