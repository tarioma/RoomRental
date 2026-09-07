namespace RoomRental.Application.Abstraction.Dto;

/// <summary>
/// Загрузка зала за период.
/// </summary>
/// <param name="RoomId">Уникальный идентификатор зала.</param>
/// <param name="RoomName">Название зала.</param>
/// <param name="Capacity">Вместимость.</param>
/// <param name="BookedHours">Забронированных часов.</param>
/// <param name="AvailableHours">Часов, доступных к бронированию за период.</param>
/// <param name="UtilizationRate">Доля занятых часов, в процентах.</param>
/// <param name="Revenue">Выручка зала за период.</param>
public record RoomUtilizationDto(
    Guid RoomId,
    string RoomName,
    int Capacity,
    decimal BookedHours,
    int AvailableHours,
    decimal UtilizationRate,
    decimal Revenue);
