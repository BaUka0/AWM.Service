namespace AWM.Service.Infrastructure.Persistence.Configurations.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for User entity.
/// Maps to [Auth].[Users] table.
/// </summary>
public class UserConfiguration : SoftDeletableEntityConfiguration<User, int>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users", "Auth");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.UniversityId)
            .IsRequired();

        builder.Property(e => e.Login)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.PasswordHash)
            .HasMaxLength(500);

        builder.Property(e => e.ExternalId)
            .HasMaxLength(255);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Foreign key to University
        builder.HasOne<University>()
            .WithMany()
            .HasForeignKey(e => e.UniversityId)
            .HasConstraintName("FK_Users_University")
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint on (UniversityId, Email)
        builder.HasIndex(e => new { e.UniversityId, e.Email })
            .IsUnique()
            .HasDatabaseName("UQ_User_Email");

        // Navigation to role assignments
        builder.HasMany(e => e.RoleAssignments)
            .WithOne()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
