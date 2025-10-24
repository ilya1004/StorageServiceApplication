using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Domain.Entities;

namespace StorageService.Infrastructure.Configurations;

public class DetailConfiguration : IEntityTypeConfiguration<Detail>
{
    public void Configure(EntityTypeBuilder<Detail> builder)
    {
        builder.ToTable("Details");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.NomenclatureCode)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Count)
            .IsRequired();

        builder.Property(d => d.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(d => d.CreatedAtDate)
            .IsRequired();

        builder.Property(d => d.DeletedAtDate)
            .IsRequired(false);

        builder.HasOne(d => d.Storekeeper)
            .WithMany(s => s.Details)
            .HasForeignKey(d => d.StorekeeperId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.NomenclatureCode);
    }
}