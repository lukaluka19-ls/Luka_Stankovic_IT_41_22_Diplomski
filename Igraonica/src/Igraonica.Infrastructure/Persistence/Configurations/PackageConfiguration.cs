using Igraonica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Igraonica.Infrastructure.Persistence.Configurations;

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.ToTable("Packages");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Type).HasConversion<short>();
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(1000).IsRequired();
        builder.Property(p => p.BasePrice).HasPrecision(10, 2);
        builder.Property(p => p.PricePerChild).HasPrecision(10, 2);
        builder.Property(p => p.IsActive).HasDefaultValue(true);
    }
}
