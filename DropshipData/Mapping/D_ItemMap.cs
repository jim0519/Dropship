using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using DropshipCommon.Models;

namespace DropshipData.Mapping
{
    public class D_ItemMap : EntityTypeConfiguration<D_Item>
    {
        public D_ItemMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.SKU)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Description)
                .IsRequired();

            this.Property(t => t.SupplierItemID)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Ref1)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Ref2)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Ref3)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Ref4)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Ref5)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.CreateBy)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.EditBy)
                .IsRequired()
                .HasMaxLength(4000);

            // Table & Column Mappings
            this.ToTable("D_Item");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.SKU).HasColumnName("SKU");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.InventoryQty).HasColumnName("InventoryQty");
            this.Property(t => t.StatusID).HasColumnName("StatusID");
            this.Property(t => t.PostageRuleID).HasColumnName("PostageRuleID");
            this.Property(t => t.SupplierID).HasColumnName("SupplierID");
            this.Property(t => t.SupplierItemID).HasColumnName("SupplierItemID");
            this.Property(t => t.Ref1).HasColumnName("Ref1");
            this.Property(t => t.Ref2).HasColumnName("Ref2");
            this.Property(t => t.Ref3).HasColumnName("Ref3");
            this.Property(t => t.Ref4).HasColumnName("Ref4");
            this.Property(t => t.Ref5).HasColumnName("Ref5");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.EditTime).HasColumnName("EditTime");
            this.Property(t => t.EditBy).HasColumnName("EditBy");
        }
    }
}
