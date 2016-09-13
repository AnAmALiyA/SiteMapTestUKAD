using System;
using System.Collections.Generic;

namespace CreateSiteMap.DAL.Entities
{
    public class History
    {
        public int Id { get; set; }
        public int PageId { get; set; }

        public DateTime Date { get; set; }

        public int ResponseTime { get; set; }
        
        public virtual Page Page { get; set; }
    }
}
