namespace RoomRental.Application.Abstraction.Dto;

/// <summary>
/// Выручка от одной услуги за период.
/// </summary>
/// <param name="ServiceId">Уникальный идентификатор услуги.</param>
/// <param name="ServiceName">Название услуги.</param>
/// <param name="Orders">Сколько раз услугу заказали.</param>
/// <param name="Revenue">Выручка от услуги.</param>
public record RevenueByServiceDto(
    Guid ServiceId,
    string ServiceName,
    int Orders,
    decimal Revenue);
