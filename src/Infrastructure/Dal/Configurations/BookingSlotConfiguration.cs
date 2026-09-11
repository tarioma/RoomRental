using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomRental.Domain.Entities;

namespace RoomRental.Dal.Configurations;

/// <summary>
/// Настройка отображения часового слота на таблицу.
/// </summary>
public class BookingSlotConfiguration : IEntityTypeConfiguration<BookingSlot>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BookingSlot> builder)
    {
        builder.HasKey(s => new { s.RoomId, s.SlotStart });

        builder.Property(s => s.BookingId).IsRequired();
        builder.Property(s => s.SlotStart).IsRequired();

        builder.HasOne(s => s.Room)
            .WithMany()
            .HasForeignKey(s => s.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.BookingId);
    }
}
