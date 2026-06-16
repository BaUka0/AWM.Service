namespace AWM.Service.Infrastructure.Persistence.Configurations.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for UserAccessHistory entity.
/// Maps to [Auth].[UserAccessHistories] table.
/// </summary>
public class UserAccessHistoryConfiguration : AuditableEntityConfiguration<UserAccessHistory, int>
{
    public override void Configure(EntityTypeBuilder<UserAccessHistory> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserAccessHistories", "Auth");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.Action)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.AssignedAt)
            .IsRequired();

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_UserAccessHistory_User");

        builder.HasIndex(e => e.RoleAccessId)
            .HasDatabaseName("IX_UserAccessHistory_Role");
    }
}
