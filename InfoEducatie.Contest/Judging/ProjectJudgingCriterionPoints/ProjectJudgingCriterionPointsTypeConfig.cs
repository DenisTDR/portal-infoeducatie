using MCMS.Base.Data.TypeConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoEducatie.Contest.Judging.ProjectJudgingCriterionPoints
{
    public class ProjectJudgingCriterionPointsTypeConfig : EntityTypeConfiguration<ProjectJudgingCriterionPointsEntity>
    {
        public override void Configure(EntityTypeBuilder<ProjectJudgingCriterionPointsEntity> builder)
        {
            base.Configure(builder);
            builder.HasOne(pj => pj.Criterion).WithMany().OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pj => pj.Project).WithMany().OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pj => pj.Judge).WithMany().OnDelete(DeleteBehavior.Cascade);
        }
    }
}