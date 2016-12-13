using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using DropshipCommon.Models;

namespace DropshipData.Mapping
{
    public class T_PostageRuleLineMap : EntityTypeConfiguration<T_PostageRuleLine>
    {
        public T_PostageRuleLineMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.PostcodeFrom)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.PostcodeTo)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.Formula)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.CreateBy)
                .IsRequired()
                .HasMaxLength(4000);

            this.Property(t => t.EditBy)
                .IsRequired()
                .HasMaxLength(4000);

            // Table & Column Mappings
            this.ToTable("T_PostageRuleLine");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.PostageRuleID).HasColumnName("PostageRuleID");
            this.Property(t => t.PostcodeFrom).HasColumnName("PostcodeFrom");
            this.Property(t => t.PostcodeTo).HasColumnName("PostcodeTo");
            this.Property(t => t.Formula).HasColumnName("Formula");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.EditTime).HasColumnName("EditTime");
            this.Property(t => t.EditBy).HasColumnName("EditBy");

            // Relationships
            this.HasRequired(t => t.T_PostageRule)
                .WithMany(t => t.T_PostageRuleLine)
                .HasForeignKey(d => d.PostageRuleID);

        }
    }
}
