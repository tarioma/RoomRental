using MediatR;

namespace RoomRental.Application.Abstraction.Commands.Rooms;

/// <summary>
/// Мягкое удаление зала: запись остаётся, но зал перестаёт участвовать в поиске и бронировании.
/// </summary>
/// <param name="RoomId">Удаляемый зал.</param>
public record DeleteRoomCommand(Guid RoomId) : IRequest;
