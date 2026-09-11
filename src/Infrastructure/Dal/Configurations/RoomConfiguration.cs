using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomRental.Domain.Entities;

namespace RoomRental.Dal.Configurations;

/// <summary>
/// Настройка отображения зала на таблицу.
/// </summary>
public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id).ValueGeneratedNever();
        builder.Property(r => r.Name).IsRequired().HasMaxLength(Room.MaxNameLength);
        builder.Property(r => r.Capacity).IsRequired();
        builder.Property(r => r.BaseHourPrice).IsRequired().HasPrecision(12, 2);
        builder.Property(r => r.CreatedAtUtc).IsRequired();
        builder.Property(r => r.DeletedAtUtc);

        builder.Ignore(r => r.IsDeleted);

        builder.HasMany(r => r.Services).WithMany();

        builder.Navigation(r => r.Services)
            .HasField("_services")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(r => r.Capacity);
        builder.HasIndex(r => r.CreatedAtUtc);
        builder.HasIndex(r => r.DeletedAtUtc);
    }
}
