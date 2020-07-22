using MCMS.Base.Data.TypeConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoEducatie.Contest.Judging.JudgingCriteria.JudgingCriteriaSection
{
    public class JudgingCriteriaSectionTypeConfig : EntityTypeConfiguration<JudgingCriteriaSectionEntity>
    {
        public override void Configure(EntityTypeBuilder<JudgingCriteriaSectionEntity> builder)
        {
            base.Configure(builder);
            builder.HasOne(s => s.Category).WithMany().OnDelete(DeleteBehavior.Restrict);
        }
    }
}