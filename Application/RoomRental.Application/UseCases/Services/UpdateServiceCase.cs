using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction;
using RoomRental.Application.Abstraction.Commands.Services;
using RoomRental.Application.Abstraction.Exceptions;
using RoomRental.Application.Abstraction.Repositories;

namespace RoomRental.Application.UseCases.Services;

/// <summary>
/// Обновляет название и стоимость услуги.
/// </summary>
/// <param name="services">Репозиторий услуг.</param>
/// <param name="unitOfWork">Фиксация изменений в базе.</param>
public class UpdateServiceCase(
    IServiceRepository services,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateServiceCommand>
{
    /// <inheritdoc />
    public async Task Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var service = await services.GetForUpdateAsync(request.ServiceId, cancellationToken)
                      ?? throw new EntityNotFoundException("Услуга не найдена.");

        service.SetName(request.Name);
        service.SetPrice(request.Price);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}