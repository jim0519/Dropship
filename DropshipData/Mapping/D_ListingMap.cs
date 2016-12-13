using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using DropshipCommon.Models;

namespace DropshipData.Mapping
{
    public class D_ListingMap : EntityTypeConfiguration<D_Listing>
    {
        public D_ListingMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties

            this.Property(t => t.ListingID)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.ListingSKU)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.ListingTitle)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.ListingDescription)
                .IsRequired();

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
            this.ToTable("D_Listing");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ItemID).HasColumnName("ItemID");
            this.Property(t => t.ListingChannelID).HasColumnName("ListingChannelID");
            //this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.ListingID).HasColumnName("ListingID");
            this.Property(t => t.ListingSKU).HasColumnName("ListingSKU");
            this.Property(t => t.ListingTitle).HasColumnName("ListingTitle");
            this.Property(t => t.ListingDescription).HasColumnName("ListingDescription");
            this.Property(t => t.ListingPrice).HasColumnName("ListingPrice");
            this.Property(t => t.ListingInventoryQty).HasColumnName("ListingInventoryQty");
            this.Property(t => t.ListingStatusID).HasColumnName("ListingStatusID");
            this.Property(t => t.ListingPriceRuleID).HasColumnName("ListingPriceRuleID");
            this.Property(t => t.ListingInventoryQtyRuleID).HasColumnName("ListingInventoryQtyRuleID");
            this.Property(t => t.ListingPostageRuleID).HasColumnName("ListingPostageRuleID");
            this.Property(t => t.ListingDescriptionTemplateID).HasColumnName("ListingDescriptionTemplateID");
            this.Property(t => t.LastUpdateTime).HasColumnName("LastUpdateTime");
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
            this.HasRequired(t => t.ListingChannel)
                .WithMany(t => t.Listings)
                .HasForeignKey(d => d.ListingChannelID);

            this.HasRequired(t => t.Item)
                .WithMany()
                .HasForeignKey(d => d.ItemID);

            this.HasRequired(t => t.ListingPriceRule)
                .WithMany()
                .HasForeignKey(d => d.ListingPriceRuleID);

            this.HasRequired(t => t.ListingInventoryQtyRule)
                .WithMany()
                .HasForeignKey(d => d.ListingInventoryQtyRuleID);

            this.HasRequired(t => t.ListingPostageRule)
                .WithMany()
                .HasForeignKey(d => d.ListingPostageRuleID);
        }
    }
}
