using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;

namespace RoomRental.Application.Abstraction.Queries.Rooms;

/// <summary>
/// Список всех действующих залов без учёта занятости.
/// Для подбора зала на конкретное время используется <see cref="SearchAvailableRoomsQuery"/>.
/// </summary>
public record SearchRoomsQuery(SearchFilters Filters) : IRequest<IReadOnlyList<RoomDto>>;
