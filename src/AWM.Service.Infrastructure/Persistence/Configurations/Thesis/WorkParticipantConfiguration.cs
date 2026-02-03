namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Edu.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for WorkParticipant entity.
/// Maps to [Thesis].[WorkParticipants] table.
/// </summary>
public class WorkParticipantConfiguration : AuditableEntityConfiguration<WorkParticipant, long>
{
    public override void Configure(EntityTypeBuilder<WorkParticipant> builder)
    {
        base.Configure(builder);

        builder.ToTable("WorkParticipants", "Thesis");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.WorkId)
            .IsRequired();

        builder.Property(e => e.StudentId)
            .IsRequired();

        builder.Property(e => e.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Ignore computed property
        builder.Ignore(e => e.JoinedAt);

        // Foreign keys
        builder.HasOne<StudentWork>()
            .WithMany(w => w.Participants)
            .HasForeignKey(e => e.WorkId)
            .HasConstraintName("FK_Participants_Work")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Student>()
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .HasConstraintName("FK_Participants_Student")
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint on (WorkId, StudentId)
        builder.HasIndex(e => new { e.WorkId, e.StudentId })
            .IsUnique()
            .HasDatabaseName("UQ_Participant_Work_Student");

        // Indexes
        builder.HasIndex(e => e.WorkId)
            .HasDatabaseName("IX_Participants_Work");

        builder.HasIndex(e => e.StudentId)
            .HasDatabaseName("IX_Participants_Student");
    }
}
