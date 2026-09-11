namespace RoomRental.Contracts.Responses;

/// <summary>
/// Загрузка зала за период: какая доля доступного времени выкуплена.
/// </summary>
/// <param name="RoomId">Уникальный идентификатор зала.</param>
/// <param name="RoomName">Название зала.</param>
/// <param name="Capacity">Вместимость в людях.</param>
/// <param name="BookedHours">Оплаченных часов по подтверждённым броням.</param>
/// <param name="AvailableHours">
/// Сколько часов зал вообще можно было сдать за период:
/// количество суток × 17 часов окна бронирования с 06:00 до 23:00.
/// </param>
/// <param name="UtilizationRate">
/// Доля выкупленных часов в процентах, с одним знаком после запятой.
/// Например 4.3 означает 4.3%.
/// </param>
/// <param name="Revenue">Выручка зала за период.</param>
public record RoomUtilizationResponse(
    Guid RoomId,
    string RoomName,
    int Capacity,
    decimal BookedHours,
    int AvailableHours,
    decimal UtilizationRate,
    decimal Revenue);
