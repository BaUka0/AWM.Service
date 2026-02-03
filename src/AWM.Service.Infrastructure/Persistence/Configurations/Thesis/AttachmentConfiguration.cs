namespace AWM.Service.Infrastructure.Persistence.Configurations.Thesis;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.Thesis.Entities;
using AWM.Service.Domain.Thesis.Enums;
using AWM.Service.Domain.Wf.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Attachment entity.
/// Maps to [Thesis].[Attachments] table.
/// </summary>
public class AttachmentConfiguration : AuditableEntityConfiguration<Attachment, long>
{
    public override void Configure(EntityTypeBuilder<Attachment> builder)
    {
        base.Configure(builder);

        builder.ToTable("Attachments", "Thesis");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.WorkId)
            .IsRequired();

        builder.Property(e => e.StateId);

        builder.Property(e => e.AttachmentType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.FileStoragePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.FileHash)
            .IsRequired()
            .HasMaxLength(64); // SHA256

        // Ignore computed properties
        builder.Ignore(e => e.UploadedBy);
        builder.Ignore(e => e.UploadedAt);

        // Foreign keys
        builder.HasOne<StudentWork>()
            .WithMany(w => w.Attachments)
            .HasForeignKey(e => e.WorkId)
            .HasConstraintName("FK_Attach_Work")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<State>()
            .WithMany()
            .HasForeignKey(e => e.StateId)
            .HasConstraintName("FK_Attach_State")
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.WorkId)
            .HasDatabaseName("IX_Attach_Work");

        builder.HasIndex(e => e.FileHash)
            .HasDatabaseName("IX_Attach_Hash");
    }
}
