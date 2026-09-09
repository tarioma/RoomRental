namespace RoomRental.Contracts.Responses;

/// <summary>
/// Строка расчёта стоимости: либо отрезок аренды по одному правилу,
/// либо одна заказанная услуга.
/// </summary>
/// <param name="Description">
/// Название правила ценообразования или услуги на момент бронирования.
/// </param>
/// <param name="Kind">Тип строки: RoomTime - аренда зала, Service - услуга.</param>
/// <param name="SegmentStart">Начало отрезка аренды. Пусто для услуг.</param>
/// <param name="SegmentEnd">Окончание отрезка аренды. Пусто для услуг.</param>
/// <param name="Quantity">Количество часов для аренды либо 1 для услуги.</param>
/// <param name="UnitPrice">
/// Цена за час либо цена услуги, зафиксированная на момент бронирования.
/// Последующее изменение прайса эту сумму не меняет.
/// </param>
/// <param name="Multiplier">
/// Множитель правила ценообразования: 1.15 - наценка 15%, 0.8 - скидка 20%.
/// Для услуг всегда 1.
/// </param>
/// <param name="Amount">Итог по строке: количество × цена × множитель.</param>
public record BookingChargeResponse(
    string Description,
    string Kind,
    DateTime? SegmentStart,
    DateTime? SegmentEnd,
    decimal Quantity,
    decimal UnitPrice,
    decimal Multiplier,
    decimal Amount);
