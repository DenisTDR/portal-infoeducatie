using MCMS.Base.Data.TypeConfig;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoEducatie.Contest.CategorySchedules;
public class CategoryScheduleEntityTypeConfig : EntityTypeConfiguration<CategoryScheduleEntity>
{
    public override void Configure(EntityTypeBuilder<CategoryScheduleEntity> builder)
    {
        base.Configure(builder);
    }
}