namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;

/// <summary>
/// EF Core configuration for ParticipantRole reference entity.
/// Maps to [Thesis].[ParticipantRoles].
/// </summary>
public class ParticipantRoleConfiguration : IEntityTypeConfiguration<ParticipantRole>
{
    public void Configure(EntityTypeBuilder<ParticipantRole> builder)
    {
        builder.ToTable("ParticipantRoles", "Thesis");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100);
    }
}
