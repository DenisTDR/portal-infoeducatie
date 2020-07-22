using MCMS.Base.Data.TypeConfig;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoEducatie.Contest.Participants.ProjectParticipant
{
    public class ProjectParticipantTypeConfig : EntityTypeConfiguration<ProjectParticipantEntity>
    {
        public override void Configure(EntityTypeBuilder<ProjectParticipantEntity> builder)
        {
            base.Configure(builder);
            builder.HasIndex("ProjectId");
            builder.HasIndex("ParticipantId");
            builder.HasIndex("ParticipantId", "ProjectId").IsUnique();
        }
    }
}