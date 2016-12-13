using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using DropshipCommon.Models;

namespace DropshipData.Mapping
{
    public class T_ListingChannelMap : EntityTypeConfiguration<T_ListingChannel>
    {
        public T_ListingChannelMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Description)
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
            this.ToTable("T_ListingChannel");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.Ref1).HasColumnName("Ref1");
            this.Property(t => t.Ref2).HasColumnName("Ref2");
            this.Property(t => t.Ref3).HasColumnName("Ref3");
            this.Property(t => t.Ref4).HasColumnName("Ref4");
            this.Property(t => t.Ref5).HasColumnName("Ref5");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.EditTime).HasColumnName("EditTime");
            this.Property(t => t.EditBy).HasColumnName("EditBy");

            // Relationships
            this.HasRequired(t => t.User)
                .WithMany(t => t.ListingChannels)
                .HasForeignKey(d => d.UserID);
        }
    }
}
