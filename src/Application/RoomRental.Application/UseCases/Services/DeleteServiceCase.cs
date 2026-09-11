using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction;
using RoomRental.Application.Abstraction.Commands.Services;
using RoomRental.Application.Abstraction.Repositories;

namespace RoomRental.Application.UseCases.Services;

/// <summary>
/// Мягко удаляет услугу.
/// </summary>
/// <param name="services">Репозиторий услуг.</param>
/// <param name="unitOfWork">Фиксация изменений в базе.</param>
/// <param name="timeProvider">Источник текущего времени.</param>
public class DeleteServiceCase(
    IServiceRepository services,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<DeleteServiceCommand>
{
    /// <inheritdoc />
    public async Task Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var service = await services.GetForUpdateAsync(request.ServiceId, cancellationToken);

        if (service is not null)
        {
            service.Delete(timeProvider.GetUtcNow().UtcDateTime);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}