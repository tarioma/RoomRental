using MediatR;

namespace RoomRental.Application.Abstraction.Commands.Rooms;

/// <summary>
/// Добавление услуги в набор доступных для зала.
/// </summary>
/// <param name="RoomId">Зал.</param>
/// <param name="ServiceId">Добавляемая услуга.</param>
public record AddServiceToRoomCommand(Guid RoomId, Guid ServiceId) : IRequest;
