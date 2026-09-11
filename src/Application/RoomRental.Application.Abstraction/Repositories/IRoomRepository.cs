using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;
using RoomRental.Domain.Entities;
using RoomRental.Domain.ValueObjects;

namespace RoomRental.Application.Abstraction.Repositories;

/// <summary>
/// Хранилище залов.
/// </summary>
public interface IRoomRepository
{
    /// <summary>
    /// Зал вместе с доступными в нём услугами, для чтения.
    /// Изменения такого экземпляра не сохраняются.
    /// </summary>
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Зал вместе с услугами, для изменения.
    /// Правки сохраняются при фиксации через <see cref="IUnitOfWork"/>.
    /// </summary>
    Task<Room?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Все действующие залы без учёта занятости.
    /// </summary>
    Task<IReadOnlyList<RoomDto>> SearchAsync(
        SearchFilters filters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Залы, свободные на весь указанный период и вмещающие не меньше <paramref name="capacity"/> человек.
    /// Зал считается занятым, если на любой час периода есть слот бронирования.
    /// </summary>
    Task<IReadOnlyList<RoomDto>> SearchAvailableAsync(
        BookingPeriod period,
        int capacity,
        SearchFilters filters,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Помечает зал к добавлению. Запись произойдёт при фиксации изменений.
    /// </summary>
    Task AddAsync(Room room, CancellationToken cancellationToken = default);
}
