namespace AWM.Service.Infrastructure.Persistence.Configurations.Wf;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Wf.Entities;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Transition entity.
/// Maps to [Wf].[Transitions] table.
/// </summary>
public class TransitionConfiguration : SoftDeletableEntityConfiguration<Transition, int>
{
    public override void Configure(EntityTypeBuilder<Transition> builder)
    {
        base.Configure(builder);

        builder.ToTable("Transitions", "Wf");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.FromStateId)
            .IsRequired();

        builder.Property(e => e.ToStateId)
            .IsRequired();

        builder.Property(e => e.AllowedRoleId);

        builder.Property(e => e.IsAutomatic)
            .IsRequired()
            .HasDefaultValue(false);

        // Foreign keys
        builder.HasOne<State>()
            .WithMany()
            .HasForeignKey(e => e.FromStateId)
            .HasConstraintName("FK_Trans_FromState")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<State>()
            .WithMany()
            .HasForeignKey(e => e.ToStateId)
            .HasConstraintName("FK_Trans_ToState")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(e => e.AllowedRoleId)
            .HasConstraintName("FK_Trans_Role")
            .OnDelete(DeleteBehavior.Restrict);

        // Index on FromStateId for transition lookup
        builder.HasIndex(e => e.FromStateId)
            .HasDatabaseName("IX_Transitions_From");
    }
}
