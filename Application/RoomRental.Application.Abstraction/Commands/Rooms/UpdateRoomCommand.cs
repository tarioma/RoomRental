using MediatR;

namespace RoomRental.Application.Abstraction.Commands.Rooms;

/// <summary>
/// Обновление данных зала. Состав услуг меняется отдельными командами.
/// </summary>
/// <param name="RoomId">Изменяемый зал.</param>
/// <param name="Name">Новое название.</param>
/// <param name="Capacity">Новая вместимость.</param>
/// <param name="BaseHourPrice">Новая базовая стоимость аренды за час.</param>
public record UpdateRoomCommand(
    Guid RoomId,
    string Name,
    int Capacity,
    decimal BaseHourPrice) : IRequest;
