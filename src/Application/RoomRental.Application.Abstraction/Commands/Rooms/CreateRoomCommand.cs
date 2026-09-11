using MediatR;

namespace RoomRental.Application.Abstraction.Commands.Rooms;

/// <summary>
/// Создание конференц-зала.
/// </summary>
/// <param name="Name">Название зала.</param>
/// <param name="Capacity">Вместимость в людях.</param>
/// <param name="BaseHourPrice">Базовая стоимость аренды за час.</param>
/// <param name="ServiceIds">Услуги, доступные при бронировании этого зала.</param>
public record CreateRoomCommand(
    string Name,
    int Capacity,
    decimal BaseHourPrice,
    IReadOnlyList<Guid> ServiceIds)
    : IRequest<Guid>;
