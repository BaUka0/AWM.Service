namespace AWM.Service.Infrastructure.Persistence.Configurations.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Auth.RbacPlus.Entities;
/// <summary>
/// EF Core configuration for RoleActionType entity.
/// Maps to [Auth].[RoleActionTypes] table.
/// </summary>
public class RoleActionTypeConfiguration : IEntityTypeConfiguration<RoleActionType>
{
    public void Configure(EntityTypeBuilder<RoleActionType> builder)
    {

        builder.ToTable("RoleActionTypes", "Auth");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.NameRu)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.NameKz)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.NameEn)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(e => e.Code)
            .IsUnique()
            .HasDatabaseName("UQ_RoleActionType_Code");
    }
}
