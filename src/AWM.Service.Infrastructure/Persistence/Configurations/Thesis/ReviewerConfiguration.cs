namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Reviewer entity.
/// Maps to [Thesis].[Reviewers] table.
/// External reviewers (not university employees).
/// </summary>
public class ReviewerConfiguration : SoftDeletableEntityConfiguration<Reviewer, int>
{
    public override void Configure(EntityTypeBuilder<Reviewer> builder)
    {
        base.Configure(builder);

        builder.ToTable("Reviewers", "Thesis");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Position)
            .HasMaxLength(200);

        builder.Property(e => e.AcademicDegree)
            .HasMaxLength(100);

        builder.Property(e => e.Organization)
            .HasMaxLength(255);

        builder.Property(e => e.Email)
            .HasMaxLength(255);

        builder.Property(e => e.Phone)
            .HasMaxLength(50);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
