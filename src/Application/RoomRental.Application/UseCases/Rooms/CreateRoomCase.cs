using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction;
using RoomRental.Application.Abstraction.Commands.Rooms;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Domain.Entities;

namespace RoomRental.Application.UseCases.Rooms;

/// <summary>
/// Создаёт зал с набором доступных услуг.
/// </summary>
/// <param name="rooms">Репозиторий залов.</param>
/// <param name="services">Репозиторий услуг.</param>
/// <param name="unitOfWork">Фиксация изменений в базе.</param>
/// <param name="timeProvider">Источник текущего времени.</param>
public class CreateRoomCase(
    IRoomRepository rooms,
    IServiceRepository services,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<CreateRoomCommand, Guid>
{
    /// <inheritdoc />
    public async Task<Guid> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);
        
        var roomServices = await services.GetAllByIdsAsync(request.ServiceIds, cancellationToken);

        var room = Room.Create(
            request.Name,
            request.Capacity,
            request.BaseHourPrice,
            roomServices,
            timeProvider.GetUtcNow().UtcDateTime);

        await rooms.AddAsync(room, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return room.Id;
    }
}