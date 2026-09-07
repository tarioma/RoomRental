using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;

namespace RoomRental.Application.Abstraction.Queries.Services;

/// <summary>
/// Список действующих услуг.
/// </summary>
/// <param name="Filters">Постраничная навигация.</param>
public record SearchServicesQuery(SearchFilters Filters) : IRequest<IReadOnlyList<ServiceDto>>;
