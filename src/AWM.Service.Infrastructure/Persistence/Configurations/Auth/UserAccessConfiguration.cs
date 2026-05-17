namespace AWM.Service.Infrastructure.Persistence.Configurations.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Auth.RbacPlus.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for UserAccess entity.
/// Maps to [Auth].[UserAccesses] table.
/// </summary>
public class UserAccessConfiguration : AuditableEntityConfiguration<UserAccess, int>
{
    public override void Configure(EntityTypeBuilder<UserAccess> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserAccesses", "Auth");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.AssignedAt)
            .IsRequired();

        builder.HasOne(e => e.User)
            .WithMany(e => e.UserAccesses)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.RoleAccess)
            .WithMany(e => e.UserAccesses)
            .HasForeignKey(e => e.RoleAccessId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.UserId, e.RoleAccessId })
            .IsUnique()
            .HasDatabaseName("UQ_UserAccess");
    }
}
