using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomRental.Domain.Entities;

namespace RoomRental.Dal.Configurations;

/// <summary>
/// Настройка отображения пользователя на таблицу.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).ValueGeneratedNever();
        builder.Property(u => u.Email).IsRequired().HasMaxLength(User.MaxEmailLength);
        builder.Property(u => u.NormalizedEmail).IsRequired().HasMaxLength(User.MaxEmailLength);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(User.MaxPasswordHashLength);
        builder.Property(u => u.Role).IsRequired().HasConversion<string>().HasMaxLength(100);
        builder.Property(u => u.CreatedAtUtc).IsRequired();
        builder.Property(u => u.DeletedAtUtc);

        builder.Ignore(u => u.IsDeleted);

        builder.HasIndex(u => u.NormalizedEmail).IsUnique();
    }
}
