namespace RoomRental.Domain.Enums;

/// <summary>
/// Тип позиции в счёте брони.
/// </summary>
public enum ChargeKind
{
    /// <summary>
    /// Значение не задано. Признак незаполненных данных.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Аренда зала за отрезок времени по одному правилу ценообразования.
    /// </summary>
    RoomTime = 1,

    /// <summary>
    /// Отдельная услуга, заказанная при бронировании.
    /// </summary>
    Service = 2
}