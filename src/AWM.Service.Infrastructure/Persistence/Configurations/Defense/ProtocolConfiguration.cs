namespace AWM.Service.Infrastructure.Persistence.Configurations.Defense;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Defense.Entities;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Protocol entity.
/// Maps to [Defense].[Protocols] table.
/// </summary>
public class ProtocolConfiguration : SoftDeletableEntityConfiguration<Protocol, long>
{
    public override void Configure(EntityTypeBuilder<Protocol> builder)
    {
        base.Configure(builder);

        builder.ToTable("Protocols", "Defense");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.ScheduleId)
            .IsRequired();

        builder.Property(e => e.CommissionId)
            .IsRequired();

        builder.Property(e => e.SessionDate)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(e => e.DocumentPath)
            .HasMaxLength(500);

        builder.Property(e => e.IsFinalized)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.FinalizedBy);

        builder.Property(e => e.FinalizedAt)
            .HasColumnType("datetime2");

        // Foreign keys
        builder.HasOne<Schedule>()
            .WithMany()
            .HasForeignKey(e => e.ScheduleId)
            .HasConstraintName("FK_Protocols_Schedule")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Commission>()
            .WithMany()
            .HasForeignKey(e => e.CommissionId)
            .HasConstraintName("FK_Protocols_Commission")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.FinalizedBy)
            .HasConstraintName("FK_Protocols_Finalizer")
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint - one protocol per schedule
        builder.HasIndex(e => e.ScheduleId)
            .IsUnique()
            .HasDatabaseName("UQ_Protocol_Schedule");
    }
}
