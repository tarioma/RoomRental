using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Queries;
using RoomRental.Application.Abstraction.Queries.Services;
using RoomRental.Application.Abstraction.Repositories;

namespace RoomRental.Application.UseCases.Services;

/// <summary>
/// Возвращает список действующих услуг постранично.
/// </summary>
/// <param name="services">Репозиторий услуг.</param>
public class SearchServicesCase(
    IServiceRepository services)
    : IRequestHandler<SearchServicesQuery, IReadOnlyList<ServiceDto>>
{
    /// <inheritdoc />
    public Task<IReadOnlyList<ServiceDto>> Handle(SearchServicesQuery request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);
        
        return services.SearchAsync(request.Filters, cancellationToken);
    }
}