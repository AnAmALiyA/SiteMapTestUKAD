using CreateSiteMap.DAL.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace CreateSiteMap.DAL.Infrastructure.Mapping
{
    public class HistoryMap : EntityTypeConfiguration<History>
    {
        public HistoryMap()
        {
            this.HasKey(t => t.Id);
            
            this.ToTable("History");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.PageId).HasColumnName("PageId");
            this.Property(t => t.ResponseTime).HasColumnName("ResponseTime");
            this.Property(t => t.Date).HasColumnName("Date");
            
            this.HasRequired(t => t.Page)
                .WithMany(t => t.History)
                .HasForeignKey(d => d.PageId);

        }
    }
}
