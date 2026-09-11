using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction;
using RoomRental.Application.Abstraction.Commands.Rooms;
using RoomRental.Application.Abstraction.Exceptions;
using RoomRental.Application.Abstraction.Repositories;

namespace RoomRental.Application.UseCases.Rooms;

/// <summary>
/// Обновляет название, вместимость и стоимость аренды зала.
/// </summary>
/// <param name="rooms">Репозиторий залов.</param>
/// <param name="unitOfWork">Фиксация изменений в базе.</param>
public class UpdateRoomCase(
    IRoomRepository rooms,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateRoomCommand>
{
    /// <inheritdoc />
    public async Task Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var room = await rooms.GetForUpdateAsync(request.RoomId, cancellationToken)
                   ?? throw new EntityNotFoundException("Зал не найден.");

        room.SetName(request.Name);
        room.SetCapacity(request.Capacity);
        room.SetBaseHourPrice(request.BaseHourPrice);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}