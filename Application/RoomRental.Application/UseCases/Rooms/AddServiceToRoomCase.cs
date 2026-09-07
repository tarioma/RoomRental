using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction;
using RoomRental.Application.Abstraction.Commands.Rooms;
using RoomRental.Application.Abstraction.Exceptions;
using RoomRental.Application.Abstraction.Repositories;

namespace RoomRental.Application.UseCases.Rooms;

/// <summary>
/// Добавляет услугу в набор доступных для зала.
/// </summary>
/// <param name="rooms">Репозиторий залов.</param>
/// <param name="services">Репозиторий услуг.</param>
/// <param name="unitOfWork">Фиксация изменений в базе.</param>
public class AddServiceToRoomCase(
    IRoomRepository rooms,
    IServiceRepository services,
    IUnitOfWork unitOfWork)
    : IRequestHandler<AddServiceToRoomCommand>
{
    /// <inheritdoc />
    public async Task Handle(AddServiceToRoomCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var room = await rooms.GetForUpdateAsync(request.RoomId, cancellationToken)
                   ?? throw new EntityNotFoundException("Зал не найден.");

        var service = await services.GetForUpdateAsync(request.ServiceId, cancellationToken)
                      ?? throw new EntityNotFoundException("Услуга не найдена.");

        room.AddService(service);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}