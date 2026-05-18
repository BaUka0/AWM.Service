namespace AWM.Service.Infrastructure.Persistence.Configurations.Defense;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Defense.Entities;

/// <summary>
/// EF Core configuration for CommissionRole reference entity.
/// Replaces RoleInCommission enum.
/// Maps to [Defense].[CommissionRoles].
/// </summary>
public class CommissionRoleConfiguration : IEntityTypeConfiguration<CommissionRole>
{
    public void Configure(EntityTypeBuilder<CommissionRole> builder)
    {
        builder.ToTable("CommissionRoles", "Defense");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100);
    }
}
