using MediatR;
using RoomRental.Application.Abstraction.Dto;

namespace RoomRental.Application.Abstraction.Queries.Rooms;

/// <summary>
/// Конференц-зал вместе с доступными в нём услугами.
/// </summary>
public record GetRoomByIdQuery(Guid RoomId) : IRequest<RoomDto>;
