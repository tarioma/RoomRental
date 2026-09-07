using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction;
using RoomRental.Application.Abstraction.Commands.Rooms;
using RoomRental.Application.Abstraction.Exceptions;
using RoomRental.Application.Abstraction.Repositories;

namespace RoomRental.Application.UseCases.Rooms;

/// <summary>
/// Исключает услугу из набора доступных для зала.
/// </summary>
/// <param name="rooms">Репозиторий залов.</param>
/// <param name="unitOfWork">Фиксация изменений в базе.</param>
public class RemoveServiceFromRoomCase(
    IRoomRepository rooms,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RemoveServiceFromRoomCommand>
{
    /// <inheritdoc />
    public async Task Handle(RemoveServiceFromRoomCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var room = await rooms.GetForUpdateAsync(request.RoomId, cancellationToken)
                   ?? throw new EntityNotFoundException("Зал не найден.");

        room.RemoveService(request.ServiceId);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}