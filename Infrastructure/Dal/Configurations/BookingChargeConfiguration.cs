using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomRental.Domain.Entities;

namespace RoomRental.Dal.Configurations;

/// <summary>
/// Настройка отображения позиции счёта на таблицу.
/// </summary>
public class BookingChargeConfiguration : IEntityTypeConfiguration<BookingCharge>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<BookingCharge> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.Kind).IsRequired().HasConversion<string>().HasMaxLength(100);
        builder.Property(c => c.Description).IsRequired().HasMaxLength(BookingCharge.MaxDescriptionLength);
        builder.Property(c => c.BookingId).IsRequired();
        builder.Property(c => c.SegmentStart);
        builder.Property(c => c.SegmentEnd);
        builder.Property(c => c.Quantity).IsRequired().HasPrecision(8, 2);
        builder.Property(c => c.UnitPrice).IsRequired().HasPrecision(12, 2);
        builder.Property(c => c.Multiplier).IsRequired().HasPrecision(6, 4);
        builder.Property(c => c.Amount).IsRequired().HasPrecision(12, 2);

        builder.HasOne(c => c.Service)
            .WithMany()
            .HasForeignKey(c => c.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.PricingRule)
            .WithMany()
            .HasForeignKey(c => c.PricingRuleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
