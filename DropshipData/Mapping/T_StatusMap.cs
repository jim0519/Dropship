using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using DropshipCommon.Models;

namespace DropshipData.Mapping
{
    public class T_StatusMap : EntityTypeConfiguration<T_Status>
    {
        public T_StatusMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.EntityType)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.CreateBy)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.EditBy)
                .IsRequired()
                .HasMaxLength(4000);

            // Table & Column Mappings
            this.ToTable("T_Status");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.EntityType).HasColumnName("EntityType");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.EditTime).HasColumnName("EditTime");
            this.Property(t => t.EditBy).HasColumnName("EditBy");
        }
    }
}
