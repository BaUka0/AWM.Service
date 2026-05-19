namespace AWM.Service.Infrastructure.Persistence.Configurations.University;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AWM.Service.Domain.University;

public class StudentStatusConfiguration : IEntityTypeConfiguration<StudentStatus>
{
    public void Configure(EntityTypeBuilder<StudentStatus> builder)
    {
        builder.ToTable("Edu_StudentStatuses");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("ID")
            .ValueGeneratedNever();

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);
    }
}
