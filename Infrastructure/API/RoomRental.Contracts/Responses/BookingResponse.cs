namespace RoomRental.Contracts.Responses;

/// <summary>
/// Подтверждение брони с полной расшифровкой расчёта стоимости.
/// </summary>
/// <param name="Id">Уникальный идентификатор брони.</param>
/// <param name="RoomId">Забронированный зал.</param>
/// <param name="RoomName">Название зала.</param>
/// <param name="Date">Дата бронирования.</param>
/// <param name="From">Время начала.</param>
/// <param name="To">Время окончания.</param>
/// <param name="Status">Состояние брони: Confirmed или Cancelled.</param>
/// <param name="TotalAmount">Итоговая стоимость - сумма всех строк расчёта.</param>
/// <param name="Charges">
/// Из чего сложилась стоимость: отрезки аренды по правилам плюс заказанные услуги.
/// </param>
public record BookingResponse(
    Guid Id,
    Guid RoomId,
    string RoomName,
    DateOnly Date,
    TimeOnly From,
    TimeOnly To,
    string Status,
    decimal TotalAmount,
    IReadOnlyList<BookingChargeResponse> Charges);
