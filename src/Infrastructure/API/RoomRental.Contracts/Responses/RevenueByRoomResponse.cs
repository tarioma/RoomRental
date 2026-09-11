namespace RoomRental.Contracts.Responses;

/// <summary>
/// Выручка одного зала за период.
/// </summary>
/// <param name="RoomId">Уникальный идентификатор зала.</param>
/// <param name="RoomName">Название зала.</param>
/// <param name="Bookings">Количество подтверждённых броней.</param>
/// <param name="Hours">Оплаченных часов аренды.</param>
/// <param name="Revenue">Выручка зала, включая заказанные услуги.</param>
public record RevenueByRoomResponse(
    Guid RoomId,
    string RoomName,
    int Bookings,
    decimal Hours,
    decimal Revenue);
