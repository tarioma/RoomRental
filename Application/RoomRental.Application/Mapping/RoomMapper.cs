using RoomRental.Application.Abstraction.Dto;
using RoomRental.Domain.Entities;

namespace RoomRental.Application.Mapping;

internal static class RoomMapper
{
    /// <summary>
    /// Превращает зал в модель ответа. Удалённые услуги в набор не попадают.
    /// </summary>
    /// <param name="room">Зал вместе с загруженными услугами.</param>
    public static RoomDto ToDto(Room room) => new(
        room.Id,
        room.Name,
        room.Capacity,
        room.BaseHourPrice,
        [.. room.Services.Where(s => !s.IsDeleted).Select(s => new ServiceDto(s.Id, s.Name, s.Price))]);
}
