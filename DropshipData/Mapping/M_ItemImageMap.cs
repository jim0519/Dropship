using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using DropshipCommon.Models;

namespace DropshipData.Mapping
{
    public class M_ItemImageMap : EntityTypeConfiguration<M_ItemImage>
    {
        public M_ItemImageMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties

            this.Property(t => t.CreateBy)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.EditBy)
                .IsRequired()
                .HasMaxLength(4000);

            // Table & Column Mappings
            this.ToTable("M_ItemImage");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ItemID).HasColumnName("ItemID");
            this.Property(t => t.ImageID).HasColumnName("ImageID");
            this.Property(t => t.DisplayOrder).HasColumnName("DisplayOrder");
            this.Property(t => t.StatusID).HasColumnName("StatusID");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.EditTime).HasColumnName("EditTime");
            this.Property(t => t.EditBy).HasColumnName("EditBy");

            // Relationships
            this.HasRequired(t => t.Image)
                .WithMany(t => t.ItemImages)
                .HasForeignKey(d => d.ImageID);
            this.HasRequired(t => t.Item)
                .WithMany(t => t.ItemImages)
                .HasForeignKey(d => d.ItemID);

        }
    }
}
