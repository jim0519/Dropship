using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using DropshipCommon.Models;

namespace DropshipData.Mapping
{
    public class T_ValueRuleLineMap : EntityTypeConfiguration<T_ValueRuleLine>
    {
        public T_ValueRuleLineMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.FieldName)
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
            this.ToTable("T_ValueRuleLine");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ValueRuleID).HasColumnName("ValueRuleID");
            this.Property(t => t.FieldName).HasColumnName("FieldName");
            this.Property(t => t.MinValue).HasColumnName("MinValue");
            this.Property(t => t.MaxValue).HasColumnName("MaxValue");
            this.Property(t => t.Formula).HasColumnName("Formula");
            this.Property(t => t.CreateTime).HasColumnName("CreateTime");
            this.Property(t => t.CreateBy).HasColumnName("CreateBy");
            this.Property(t => t.EditTime).HasColumnName("EditTime");
            this.Property(t => t.EditBy).HasColumnName("EditBy");

            // Relationships
            this.HasRequired(t => t.T_ValueRule)
                .WithMany(t => t.T_ValueRuleLine)
                .HasForeignKey(d => d.ValueRuleID);

        }
    }
}
