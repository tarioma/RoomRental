using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomRental.Domain.Entities;

namespace RoomRental.Dal.Configurations;

/// <summary>
/// Настройка отображения правила ценообразования на таблицу.
/// </summary>
public class PricingRuleConfiguration : IEntityTypeConfiguration<PricingRule>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PricingRule> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(PricingRule.MaxNameLength);
        builder.Property(p => p.From).IsRequired();
        builder.Property(p => p.To).IsRequired();
        builder.Property(p => p.Multiplier).IsRequired().HasPrecision(6, 4);
        builder.Property(p => p.Priority).IsRequired();
        builder.Property(p => p.IsActive).IsRequired();

        builder.HasIndex(p => p.IsActive);
    }
}
