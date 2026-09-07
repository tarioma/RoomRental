using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction;
using RoomRental.Application.Abstraction.Commands.Rooms;
using RoomRental.Application.Abstraction.Repositories;

namespace RoomRental.Application.UseCases.Rooms;

/// <summary>
/// Мягко удаляет зал.
/// </summary>
/// <param name="rooms">Репозиторий залов.</param>
/// <param name="unitOfWork">Фиксация изменений в базе.</param>
/// <param name="timeProvider">Источник текущего времени.</param>
public class DeleteRoomCase(
    IRoomRepository rooms,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<DeleteRoomCommand>
{
    /// <inheritdoc />
    public async Task Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var room = await rooms.GetForUpdateAsync(request.RoomId, cancellationToken);

        if (room is not null)
        {
            room.Delete(timeProvider.GetUtcNow().UtcDateTime);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}