using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomRental.Domain.Entities;

namespace RoomRental.Dal.Configurations;

/// <summary>
/// Настройка отображения услуги на таблицу.
/// </summary>
public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property(s => s.Name).IsRequired().HasMaxLength(Service.MaxNameLength);
        builder.Property(s => s.Price).IsRequired().HasPrecision(12, 2);
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.DeletedAtUtc);

        builder.Ignore(s => s.IsDeleted);

        builder.HasIndex(s => s.CreatedAtUtc);
        builder.HasIndex(s => s.DeletedAtUtc);
    }
}
