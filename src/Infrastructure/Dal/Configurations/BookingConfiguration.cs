using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomRental.Domain.Entities;

namespace RoomRental.Dal.Configurations;

/// <summary>
/// Настройка отображения брони на таблицу.
/// </summary>
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id).ValueGeneratedNever();
        builder.Property(b => b.TotalAmount).IsRequired().HasPrecision(12, 2);
        builder.Property(b => b.Status).IsRequired().HasConversion<string>().HasMaxLength(100);
        builder.Property(b => b.CreatedAtUtc).IsRequired();
        builder.Property(b => b.CancelledAtUtc);
        builder.Property(b => b.CancellationReason).HasMaxLength(Booking.MaxCancellationReasonLength);

        builder.ComplexProperty(b => b.Period, period =>
        {
            period.Property(p => p.Date).HasColumnName("date").IsRequired();
            period.Property(p => p.From).HasColumnName("start_time").IsRequired();
            period.Property(p => p.To).HasColumnName("end_time").IsRequired();
            period.Ignore(p => p.StartsAtUtc);
            period.Ignore(p => p.EndsAtUtc);
            period.Ignore(p => p.Hours);
        });

        builder.HasOne(b => b.Room)
            .WithMany()
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Customer)
            .WithMany()
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.Charges)
            .WithOne(c => c.Booking)
            .HasForeignKey(c => c.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(b => b.Charges)
            .HasField("_charges")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(b => b.Slots)
            .WithOne(s => s.Booking)
            .HasForeignKey(s => s.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(b => b.Slots)
            .HasField("_slots")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(b => b.RoomId);
        builder.HasIndex(b => b.CustomerId);

        // Индекс (status, date) под отчёты создаётся вручную в миграции:
        // EF не умеет строить индексы по свойствам комплексного типа.
        // См. ReportIndexes.
    }
}
