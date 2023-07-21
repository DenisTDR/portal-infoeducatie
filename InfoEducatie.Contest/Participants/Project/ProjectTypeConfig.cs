using MCMS.Base.Data.TypeConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfoEducatie.Contest.Participants.Project
{
    public class ProjectTypeConfig : EntityTypeConfiguration<ProjectEntity>
    {
        public override void Configure(EntityTypeBuilder<ProjectEntity> builder)
        {
            base.Configure(builder);
            builder
                .HasMany(p => p.Participants)
                .WithMany(p => p.Projects)
                .UsingEntity(e => e.ToTable("ProjectParticipants"));
            builder.HasIndex(p => p.Disabled);
        }
    }
}