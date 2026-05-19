namespace AWM.Service.Infrastructure.Persistence.Configurations.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for LocalAccount entity.
/// Maps to [Auth].[LocalAccounts] table.
/// </summary>
public class LocalAccountConfiguration : AuditableEntityConfiguration<LocalAccount, int>
{
    public override void Configure(EntityTypeBuilder<LocalAccount> builder)
    {
        base.Configure(builder);

        builder.ToTable("LocalAccounts", "Auth");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.RefreshToken)
            .HasMaxLength(500);

        builder.Property(e => e.RefreshTokenExpiryTime)
            .HasColumnType("datetime2");

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // One-to-one relationship with read-only University User
        builder.HasOne(e => e.User)
            .WithOne()
            .HasForeignKey<LocalAccount>(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique index on UserId
        builder.HasIndex(e => e.UserId)
            .IsUnique()
            .HasDatabaseName("UQ_LocalAccount_UserId");
    }
}
