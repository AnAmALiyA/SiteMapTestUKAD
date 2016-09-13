using CreateSiteMap.DAL.Entities;
using CreateSiteMap.DAL.Infrastructure.Mapping;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace CreateSiteMap.DAL.EF
{
    public class SiteMapContext : DbContext
    {
        static SiteMapContext()
        {
            Database.SetInitializer<SiteMapContext>(null);
        }

        public SiteMapContext()
            : base("DatabaseSiteMap")
        {
            this.Configuration.ProxyCreationEnabled = false;            
        }

        public DbSet<Host> Hosts { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<History> History { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {            
            modelBuilder.Configurations.Add(new HistoryMap());
            modelBuilder.Configurations.Add(new HostMap());
            modelBuilder.Configurations.Add(new PageMap());
        }
    }
}
