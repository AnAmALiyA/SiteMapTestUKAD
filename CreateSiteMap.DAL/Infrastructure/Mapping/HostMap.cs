using CreateSiteMap.DAL.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CreateSiteMap.DAL.Infrastructure.Mapping
{
    public class HostMap : EntityTypeConfiguration<Host>
    {
        public HostMap()
        {
            this.HasKey(t => t.Id);
            
            this.Property(t => t.HostName)
                .IsRequired()
                .HasMaxLength(250);
            
            this.ToTable("Hosts");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.HostName).HasColumnName("Host");
        }
    }
}
