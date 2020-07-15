using MCMS.Base.Data.TypeConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoEducatie.Contest.Judging.JudgingCriteria
{
    public class JudgingCriterionTypeConfig : EntityTypeConfiguration<JudgingCriterionEntity>
    {
        public override void Configure(EntityTypeBuilder<JudgingCriterionEntity> builder)
        {
            base.Configure(builder);
            builder.HasOne(jc => jc.Category).WithMany().OnDelete(DeleteBehavior.Cascade);
        }
    }
}