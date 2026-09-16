using Igraonica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Igraonica.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.PhoneNumber).HasMaxLength(30).IsRequired();
        builder.Property(u => u.Role).HasConversion<short>();
        builder.Property(u => u.IsEmailVerified).HasDefaultValue(false);
        builder.Property(u => u.EmailVerificationToken).HasMaxLength(128);
        builder.Property(u => u.PasswordResetToken).HasMaxLength(128);
        builder.Property(u => u.CreatedAt).IsRequired();
    }
}
