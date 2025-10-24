using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Domain.Entities;

namespace StorageService.Infrastructure.Configurations;

public class StorekeeperConfiguration : IEntityTypeConfiguration<Storekeeper>
{
    public void Configure(EntityTypeBuilder<Storekeeper> builder)
    {
        builder.ToTable("Storekeepers");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);
    }
}