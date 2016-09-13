using CreateSiteMap.DAL.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CreateSiteMap.DAL.Infrastructure.Mapping
{
    public class PageMap : EntityTypeConfiguration<Page>
    {
        public PageMap()
        {            
            this.HasKey(t => t.Id);
                       
            this.Property(t => t.Url)
                .IsRequired()
                .HasMaxLength(400);
                       
            this.ToTable("Pages");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.HostId).HasColumnName("HostId");
            this.Property(t => t.Url).HasColumnName("Url");
            this.Property(t => t.MinResponseTime).HasColumnName("MinResponseTime");
            this.Property(t => t.MaxResponseTime).HasColumnName("MaxResponseTime");
                       
            this.HasRequired(t => t.Host)
                .WithMany(t => t.Pages)
                .HasForeignKey(d => d.HostId);
        }
    }
}
