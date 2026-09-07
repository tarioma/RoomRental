using RoomRental.Domain.Entities;
using RoomRental.Domain.ValueObjects;

namespace RoomRental.Application.Abstraction.Repositories;

/// <summary>
/// Хранилище броней.
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Бронь вместе с залом и позициями счёта, для чтения.
    /// Изменения такого экземпляра не сохраняются.
    /// </summary>
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Бронь вместе с позициями счёта и часовыми слотами, для изменения.
    /// Слоты обязательно загружены: при отмене они удаляются,
    /// и без них освобождение часов не дойдёт до базы.
    /// </summary>
    Task<Booking?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Занят ли зал хотя бы на один час указанного периода.
    /// </summary>
    Task<bool> HasOverlappingSlotsAsync(
        Guid roomId,
        BookingPeriod period,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Помечает бронь к добавлению вместе с позициями счёта и часовыми слотами.
    /// </summary>
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
}
