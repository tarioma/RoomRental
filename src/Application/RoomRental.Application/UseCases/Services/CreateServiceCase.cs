using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction;
using RoomRental.Application.Abstraction.Commands.Services;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Domain.Entities;

namespace RoomRental.Application.UseCases.Services;

/// <summary>
/// Создаёт услугу.
/// </summary>
/// <param name="services">Репозиторий услуг.</param>
/// <param name="unitOfWork">Фиксация изменений в базе.</param>
/// <param name="timeProvider">Источник текущего времени.</param>
public class CreateServiceCase(
    IServiceRepository services,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : IRequestHandler<CreateServiceCommand, Guid>
{
    /// <inheritdoc />
    public async Task<Guid> Handle(CreateServiceCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var service = Service.Create(request.Name, request.Price, timeProvider.GetUtcNow().UtcDateTime);

        await services.AddAsync(service, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return service.Id;
    }
}