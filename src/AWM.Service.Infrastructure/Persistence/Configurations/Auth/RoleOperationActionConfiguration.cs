namespace AWM.Service.Infrastructure.Persistence.Configurations.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Auth.Entities;
/// <summary>
/// EF Core configuration for RoleOperationAction entity.
/// Maps to [Auth].[RoleOperationActions] table.
/// </summary>
public class RoleOperationActionConfiguration : IEntityTypeConfiguration<RoleOperationAction>
{
    public void Configure(EntityTypeBuilder<RoleOperationAction> builder)
    {

        builder.ToTable("RoleOperationActions", "Auth");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.HasOne(e => e.RoleAccess)
            .WithMany(e => e.OperationActions)
            .HasForeignKey(e => e.RoleAccessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.RoleOperation)
            .WithMany(e => e.OperationActions)
            .HasForeignKey(e => e.RoleOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.RoleActionType)
            .WithMany(e => e.OperationActions)
            .HasForeignKey(e => e.RoleActionTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.RoleAccessId, e.RoleOperationId, e.RoleActionTypeId })
            .IsUnique()
            .HasDatabaseName("UQ_RoleOperationAction");
    }
}
