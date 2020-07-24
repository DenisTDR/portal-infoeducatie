using MCMS.Base.Data.TypeConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoEducatie.Contest.Judging.Judges
{
    public class JudgeTypeConfig : EntityTypeConfiguration<JudgeEntity>
    {
        public override void Configure(EntityTypeBuilder<JudgeEntity> builder)
        {
            base.Configure(builder);
            builder.HasOne(j => j.User).WithMany().OnDelete(DeleteBehavior.Cascade);
        }
    }
}