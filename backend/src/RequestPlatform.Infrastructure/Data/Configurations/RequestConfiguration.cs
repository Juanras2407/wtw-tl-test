using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RequestPlatform.Domain.Entities;

namespace RequestPlatform.Infrastructure.Data.Configurations;

public class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.ToTable("Requests");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.Type)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.DynamicData)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        // ISJSON check constraint for SQL Server
        builder.ToTable(t => t.HasCheckConstraint(
            "CK_Requests_DynamicData_IsJson",
            "ISJSON(DynamicData) = 1"));

        // Indexes for filtering performance
        builder.HasIndex(r => r.Type);
        builder.HasIndex(r => r.Status);
    }
}
