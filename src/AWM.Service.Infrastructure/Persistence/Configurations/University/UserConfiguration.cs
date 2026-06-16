namespace AWM.Service.Infrastructure.Persistence.Configurations.University;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.University;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Edu_Users", t => t.ExcludeFromMigrations());
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.FirstName)
            .HasMaxLength(200);

        builder.Property(e => e.MiddleName)
            .HasMaxLength(200);

        builder.Property(e => e.IIN)
            .HasMaxLength(20);

        builder.Property(e => e.Email)
            .HasMaxLength(256);

        builder.Property(e => e.DOB)
            .HasColumnType("datetime");

        builder.Property(e => e.Male);

        builder.Property(e => e.MobilePhone)
            .HasMaxLength(50);

        builder.Property(e => e.PhotoFileName)
            .HasMaxLength(256);
    }
}
