using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;

namespace RoomRental.Application.Abstraction.Queries.Rooms;

/// <summary>
/// Поиск залов, свободных на указанный период и вмещающих нужное количество человек.
/// </summary>
/// <param name="Date">Дата бронирования.</param>
/// <param name="From">Время начала.</param>
/// <param name="To">Время окончания.</param>
/// <param name="Capacity">Минимально необходимая вместимость.</param>
/// <param name="Filters">Постраничная навигация.</param>
public record SearchAvailableRoomsQuery(
    DateOnly Date,
    TimeOnly From,
    TimeOnly To,
    int Capacity,
    SearchFilters Filters)
    : IRequest<IReadOnlyList<RoomDto>>;
