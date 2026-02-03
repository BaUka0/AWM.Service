namespace AWM.Service.Infrastructure.Persistence.Configurations.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Domain.Auth.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for Notification entity.
/// Maps to [Common].[Notifications] table.
/// </summary>
public class NotificationConfiguration : AuditableEntityConfiguration<Notification, long>
{
    public override void Configure(EntityTypeBuilder<Notification> builder)
    {
        base.Configure(builder);

        builder.ToTable("Notifications", "Common");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.TemplateId);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Body)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.RelatedEntityType)
            .HasMaxLength(50);

        builder.Property(e => e.RelatedEntityId);

        builder.Property(e => e.IsRead)
            .IsRequired()
            .HasDefaultValue(false);

        // Foreign keys
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<NotificationTemplate>()
            .WithMany()
            .HasForeignKey(e => e.TemplateId)
            .HasConstraintName("FK_Notif_Template")
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(e => new { e.UserId, e.IsRead, e.CreatedAt })
            .HasDatabaseName("IX_Notif_User_Unread")
            .IsDescending(false, false, true);

        builder.HasIndex(e => new { e.RelatedEntityType, e.RelatedEntityId })
            .HasDatabaseName("IX_Notif_Entity");
    }
}
