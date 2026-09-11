using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Exceptions;
using RoomRental.Application.Abstraction.Queries.Rooms;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Application.Mapping;

namespace RoomRental.Application.UseCases.Rooms;

/// <summary>
/// Возвращает зал вместе с доступными в нём услугами.
/// </summary>
/// <param name="rooms">Репозиторий залов.</param>
public class GetRoomByIdCase(
    IRoomRepository rooms)
    : IRequestHandler<GetRoomByIdQuery, RoomDto>
{
    /// <inheritdoc />
    public async Task<RoomDto> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var room = await rooms.GetByIdAsync(request.RoomId, cancellationToken)
                   ?? throw new EntityNotFoundException("Зал не найден.");

        return RoomMapper.ToDto(room);
    }
}
