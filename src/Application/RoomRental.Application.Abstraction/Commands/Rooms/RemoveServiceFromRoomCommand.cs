using MediatR;

namespace RoomRental.Application.Abstraction.Commands.Rooms;

/// <summary>
/// Исключение услуги из набора доступных для зала.
/// Уже созданные брони это не затрагивает.
/// </summary>
/// <param name="RoomId">Зал.</param>
/// <param name="ServiceId">Исключаемая услуга.</param>
public record RemoveServiceFromRoomCommand(Guid RoomId, Guid ServiceId) : IRequest;
