using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using DropshipCommon.Models;

namespace DropshipData.Mapping
{
    public class D_ImageMap : EntityTypeConfiguration<D_Image>
    {
        public D_ImageMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ImagePath)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.CreateBy)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.EditBy)
                .IsRequired()
                .HasMaxLength(4000);

            // Table & Column Mappings
            this.ToTable("D_Image");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ImagePath).HasColumnName("ImagePath");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.EditTime).HasColumnName("EditTime");
            this.Property(t => t.EditBy).HasColumnName("EditBy");
        }
    }
}
