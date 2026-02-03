namespace AWM.Service.Infrastructure.Persistence.Configurations.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Domain.Org.Entities;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for UserRoleAssignment entity.
/// Maps to [Auth].[UserRoleAssignments] table.
/// Implements Context-Aware RBAC.
/// </summary>
public class UserRoleAssignmentConfiguration : AuditableEntityConfiguration<UserRoleAssignment, long>
{
    public override void Configure(EntityTypeBuilder<UserRoleAssignment> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserRoleAssignments", "Auth");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.RoleId)
            .IsRequired();

        builder.Property(e => e.DepartmentId);

        builder.Property(e => e.InstituteId);

        builder.Property(e => e.AcademicYearId);

        builder.Property(e => e.ValidFrom)
            .HasColumnType("datetime2");

        builder.Property(e => e.ValidTo)
            .HasColumnType("datetime2");

        // Ignore computed property
        builder.Ignore(e => e.AssignedBy);

        // Foreign keys
        builder.HasOne<User>()
            .WithMany(u => u.RoleAssignments)
            .HasForeignKey(e => e.UserId)
            .HasConstraintName("FK_URA_User")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(e => e.RoleId)
            .HasConstraintName("FK_URA_Role")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Department>()
            .WithMany()
            .HasForeignKey(e => e.DepartmentId)
            .HasConstraintName("FK_URA_Dept")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Institute>()
            .WithMany()
            .HasForeignKey(e => e.InstituteId)
            .HasConstraintName("FK_URA_Institute")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AcademicYear>()
            .WithMany()
            .HasForeignKey(e => e.AcademicYearId)
            .HasConstraintName("FK_URA_Year")
            .OnDelete(DeleteBehavior.Restrict);

        // Index for context-aware queries
        builder.HasIndex(e => new { e.UserId, e.DepartmentId })
            .HasDatabaseName("IX_URA_UserCtx")
            .HasFilter("[DepartmentId] IS NOT NULL");
    }
}
