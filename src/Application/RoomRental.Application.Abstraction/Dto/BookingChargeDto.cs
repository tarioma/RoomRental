using RoomRental.Domain.Enums;

namespace RoomRental.Application.Abstraction.Dto;

/// <summary>
/// Позиция в счёте брони: отрезок аренды по одному правилу либо заказанная услуга.
/// </summary>
/// <param name="Description">Название правила или услуги.</param>
/// <param name="Kind">Тип позиции.</param>
/// <param name="SegmentStart">Начало отрезка аренды, только для аренды зала.</param>
/// <param name="SegmentEnd">Окончание отрезка аренды, только для аренды зала.</param>
/// <param name="Quantity">Количество часов либо 1 для услуги.</param>
/// <param name="UnitPrice">Цена за час либо цена услуги на момент брони.</param>
/// <param name="Multiplier">Множитель правила ценообразования.</param>
/// <param name="Amount">Итог по позиции.</param>
public record BookingChargeDto(
    string Description,
    ChargeKind Kind,
    DateTime? SegmentStart,
    DateTime? SegmentEnd,
    decimal Quantity,
    decimal UnitPrice,
    decimal Multiplier,
    decimal Amount);
