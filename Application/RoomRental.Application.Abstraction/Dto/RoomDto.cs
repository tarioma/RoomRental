namespace RoomRental.Application.Abstraction.Dto;

/// <summary>
/// Конференц-зал в ответах API.
/// </summary>
/// <param name="Id">Уникальный идентификатор.</param>
/// <param name="Name">Название.</param>
/// <param name="Capacity">Вместимость.</param>
/// <param name="BaseHourPrice">Базовая стоимость аренды за час.</param>
/// <param name="Services">Услуги, доступные в зале.</param>
public record RoomDto(
    Guid Id,
    string Name,
    int Capacity,
    decimal BaseHourPrice,
    IReadOnlyList<ServiceDto> Services);
