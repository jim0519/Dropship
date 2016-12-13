using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using DropshipCommon.Models;

namespace DropshipData.Mapping
{
    public class T_CategoryMap : EntityTypeConfiguration<T_Category>
    {
        public T_CategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.CategoryID)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.CategoryName)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.ParentCategoryID)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.CreateBy)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.EditBy)
                .IsRequired()
                .HasMaxLength(4000);

            // Table & Column Mappings
            this.ToTable("T_Category");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");
            this.Property(t => t.CategoryName).HasColumnName("CategoryName");
            this.Property(t => t.ParentCategoryID).HasColumnName("ParentCategoryID");
            this.Property(t => t.SupplierID).HasColumnName("SupplierID");
            this.Property(t => t.StatusID).HasColumnName("StatusID");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.EditTime).HasColumnName("EditTime");
            this.Property(t => t.EditBy).HasColumnName("EditBy");
        }
    }
}
