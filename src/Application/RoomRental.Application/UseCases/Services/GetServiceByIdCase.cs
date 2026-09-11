using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Queries;
using RoomRental.Application.Abstraction.Queries.Services;
using RoomRental.Application.Abstraction.Repositories;

namespace RoomRental.Application.UseCases.Services;

/// <summary>
/// Возвращает услугу по идентификатору.
/// </summary>
/// <param name="services">Репозиторий услуг.</param>
public class GetServiceByIdCase(
    IServiceRepository services)
    : IRequestHandler<GetServiceByIdQuery, ServiceDto?>
{
    /// <inheritdoc />
    public async Task<ServiceDto?> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);

        var service = await services.GetByIdAsync(request.ServiceId, cancellationToken);

        return service is null
            ? null
            : new ServiceDto(service.Id, service.Name, service.Price);
    }
}