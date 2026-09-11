using Ardalis.GuardClauses;
using RoomRental.Domain.Enums;

namespace RoomRental.Domain.Entities;

/// <summary>
/// Расчёт позиции в счёте. Одна бронь порождает несколько таких строк.
/// </summary>
public class BookingCharge
{
    /// <summary>
    /// Предельная длина описания позиции.
    /// </summary>
    public const int MaxDescriptionLength = 200;

    /// <summary>
    /// Описание для часов, не покрытых ни одним правилом ценообразования.
    /// Отдельное название нужно, чтобы такие часы не сливались в счёте и отчётах
    /// с часами настоящего правила.
    /// </summary>
    public const string BaseRateDescription = "Базовая ставка";

    private BookingCharge(
        Guid id,
        ChargeKind kind,
        string description,
        Guid bookingId,
        Guid? serviceId,
        Guid? pricingRuleId,
        DateTime? segmentStart,
        DateTime? segmentEnd,
        decimal quantity,
        decimal unitPrice,
        decimal multiplier)
    {
        Guard.Against.Default(id);
        Guard.Against.Default(kind);
        Guard.Against.EnumOutOfRange(kind);
        Guard.Against.NullOrWhiteSpace(description);
        Guard.Against.StringTooLong(description, MaxDescriptionLength);
        Guard.Against.Default(bookingId);
        Guard.Against.NegativeOrZero(quantity);
        Guard.Against.Negative(unitPrice);
        Guard.Against.NegativeOrZero(multiplier);

        Id = id;
        Kind = kind;
        Description = description.Trim();
        BookingId = bookingId;
        ServiceId = serviceId;
        PricingRuleId = pricingRuleId;
        SegmentStart = segmentStart;
        SegmentEnd = segmentEnd;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Multiplier = multiplier;
        Amount = Math.Round(quantity * unitPrice * multiplier, 2, MidpointRounding.AwayFromZero);
    }

    private BookingCharge()
    {
    }
    
    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Тип расчёта.
    /// </summary>
    public ChargeKind Kind { get; }

    /// <summary>
    /// Описание.
    /// </summary>
    public string Description { get; } = null!;
    
    /// <summary>
    /// Уникальный идентификатор бронирования.
    /// </summary>
    public Guid BookingId { get; }

    /// <summary>
    /// Бронирование.
    /// </summary>
    public Booking Booking { get; } = null!;

    /// <summary>
    /// Уникальный идентификатор услуги при бронировании.
    /// Только для <see cref="Kind"/> == <see cref="ChargeKind.Service"/>.
    /// </summary>
    public Guid? ServiceId { get; }

    /// <summary>
    /// Услуга при бронировании.
    /// Только для <see cref="Kind"/> == <see cref="ChargeKind.Service"/>.
    /// </summary>
    public Service? Service { get; } = null!;

    /// <summary>
    /// Уникальный идентификатор правила бронирования в определённое время суток.
    /// Только для <see cref="Kind"/> == <see cref="ChargeKind.RoomTime"/>.
    /// </summary>
    public Guid? PricingRuleId { get; }

    /// <summary>
    /// Правило бронирования в определённое время суток.
    /// Только для <see cref="Kind"/> == <see cref="ChargeKind.RoomTime"/>.
    /// </summary>
    public PricingRule? PricingRule { get; } = null!;

    /// <summary>
    /// Начало диапазона.
    /// Только для <see cref="Kind"/> == <see cref="ChargeKind.RoomTime"/>.
    /// </summary>
    public DateTime? SegmentStart { get; }

    /// <summary>
    /// Конец диапазона.
    /// Только для <see cref="Kind"/> == <see cref="ChargeKind.RoomTime"/>.
    /// </summary>
    public DateTime? SegmentEnd { get; }

    /// <summary>
    /// Количество.
    /// Для <see cref="Kind"/> == <see cref="ChargeKind.RoomTime"/> значение равно количеству часов.
    /// Для <see cref="Kind"/> == <see cref="ChargeKind.Service"/> значение равно 1.
    /// </summary>
    public decimal Quantity { get; }

    /// <summary>
    /// Цена за единицу.
    /// Для <see cref="Kind"/> == <see cref="ChargeKind.RoomTime"/> значение равно цене в час.
    /// Для <see cref="Kind"/> == <see cref="ChargeKind.Service"/> значение равно цене услуги.
    /// </summary>
    public decimal UnitPrice { get; }

    /// <summary>
    /// Множитель.
    /// Для <see cref="Kind"/> == <see cref="ChargeKind.RoomTime"/> значение равно множителю
    /// правила бронирования в определённое время суток.
    /// Для <see cref="Kind"/> == <see cref="ChargeKind.Service"/> значение равно 1.
    /// </summary>
    public decimal Multiplier { get; }

    /// <summary>
    /// Итоговая стоимость данной позиции в счёте.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Создаёт позицию за отрезок аренды зала по одному правилу ценообразования.
    /// </summary>
    /// <param name="bookingId">Бронь, к которой относится позиция.</param>
    /// <param name="segmentStart">Начало отрезка.</param>
    /// <param name="segmentEnd">Окончание отрезка.</param>
    /// <param name="baseHourPrice">Базовая стоимость аренды за час.</param>
    /// <param name="rule">Действующее правило. Без правила применяется базовая ставка.</param>
    public static BookingCharge ForRoomTime(
        Guid bookingId,
        DateTime segmentStart,
        DateTime segmentEnd,
        decimal baseHourPrice,
        PricingRule? rule)
    {
        Guard.Against.Default(bookingId);
        Guard.Against.Default(segmentStart);
        Guard.Against.Default(segmentEnd);
        Guard.Against.NegativeOrZero(baseHourPrice);

        return new BookingCharge(
            id: Guid.CreateVersion7(),
            kind: ChargeKind.RoomTime,
            description: rule?.Name ?? BaseRateDescription,
            bookingId,
            serviceId: null,
            pricingRuleId: rule?.Id,
            segmentStart,
            segmentEnd,
            quantity: (decimal)(segmentEnd - segmentStart).Ticks / TimeSpan.TicksPerHour,
            unitPrice: baseHourPrice,
            multiplier: rule?.Multiplier ?? 1m);
    }

    /// <summary>
    /// Создаёт позицию за одну заказанную услугу.
    /// Цена копируется в позицию, поэтому её последующее изменение не переписывает бронь.
    /// </summary>
    /// <param name="service">Заказанная услуга.</param>
    /// <param name="bookingId">Бронь, к которой относится позиция.</param>
    public static BookingCharge ForService(Service service, Guid bookingId)
    {
        Guard.Against.Null(service);
        Guard.Against.Default(bookingId);

        return new BookingCharge(
            id: Guid.CreateVersion7(),
            kind: ChargeKind.Service,
            description: service.Name,
            bookingId,
            serviceId: service.Id,
            pricingRuleId: null,
            segmentStart: null,
            segmentEnd: null,
            quantity: 1m,
            unitPrice: service.Price,
            multiplier: 1m);
    }
}