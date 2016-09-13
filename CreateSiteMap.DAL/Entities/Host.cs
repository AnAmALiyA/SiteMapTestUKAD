using System.Collections.Generic;

namespace CreateSiteMap.DAL.Entities
{
    public class Host
    {
        public Host()
        {
            this.Pages = new List<Page>();
        }

        public int Id { get; set; }
        public string HostName { get; set; }
        public virtual ICollection<Page> Pages { get; set; }
    }
}
