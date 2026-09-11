using RoomRental.Domain.Entities;
using RoomRental.Domain.ValueObjects;

namespace RoomRental.Domain.Services;

/// <summary>
/// Разбивает период аренды на сегменты по правилам ценообразования
/// и считает стоимость аренды зала за каждый сегмент.
/// </summary>
public static class RoomTimePricing
{
    /// <summary>
    /// Возвращает позиции счёта за аренду зала.
    /// Соседние часы с одним и тем же правилом склеиваются в один сегмент.
    /// </summary>
    public static IReadOnlyList<BookingCharge> Split(
        Guid bookingId,
        BookingPeriod period,
        decimal baseHourPrice,
        IReadOnlyList<PricingRule> rules)
    {
        var charges = new List<BookingCharge>();
        var segmentStart = period.StartsAtUtc;
        var currentRule = RuleFor(segmentStart, rules);

        for (var hour = segmentStart.AddHours(1); hour < period.EndsAtUtc; hour = hour.AddHours(1))
        {
            var rule = RuleFor(hour, rules);

            if (rule?.Id == currentRule?.Id)
            {
                continue;
            }

            charges.Add(BookingCharge.ForRoomTime(bookingId, segmentStart, hour, baseHourPrice, currentRule));
            segmentStart = hour;
            currentRule = rule;
        }

        charges.Add(BookingCharge.ForRoomTime(bookingId, segmentStart, period.EndsAtUtc, baseHourPrice, currentRule));

        return charges;
    }

    /// <summary>
    /// Правило, действующее в указанный час. Побеждает правило с большим приоритетом.
    /// Если ни одно правило не подходит, применяется базовая ставка.
    /// </summary>
    private static PricingRule? RuleFor(DateTime hour, IReadOnlyList<PricingRule> rules)
    {
        var time = TimeOnly.FromDateTime(hour);

        return rules
            .Where(r => r.IsActive && time >= r.From && time < r.To)
            .OrderByDescending(r => r.Priority)
            .FirstOrDefault();
    }
}