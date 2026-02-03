namespace AWM.Service.Infrastructure.Persistence.Configurations.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.CommonDomain.Entities;
using AWM.Service.Infrastructure.Persistence.Configurations.Base;

/// <summary>
/// EF Core configuration for NotificationTemplate entity.
/// Maps to [Common].[NotificationTemplates] table.
/// </summary>
public class NotificationTemplateConfiguration : SoftDeletableEntityConfiguration<NotificationTemplate, int>
{
    public override void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        base.Configure(builder);

        builder.ToTable("NotificationTemplates", "Common");

        builder.Property(e => e.Id)
            .UseIdentityColumn();

        builder.Property(e => e.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.TitleRu)
            .HasMaxLength(255);

        builder.Property(e => e.TitleKz)
            .HasMaxLength(255);

        builder.Property(e => e.TitleEn)
            .HasMaxLength(255);

        builder.Property(e => e.BodyTemplateRu)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.BodyTemplateKz)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.BodyTemplateEn)
            .HasColumnType("nvarchar(max)");

        // Unique constraint on EventType
        builder.HasIndex(e => e.EventType)
            .IsUnique()
            .HasDatabaseName("UQ_Template_Event");
    }
}
