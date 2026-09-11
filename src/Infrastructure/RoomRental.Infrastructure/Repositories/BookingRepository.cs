using Microsoft.EntityFrameworkCore;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Dal;
using RoomRental.Domain.Entities;
using RoomRental.Domain.ValueObjects;

namespace RoomRental.Infrastructure.Repositories;

/// <summary>
/// Хранилище броней.
/// </summary>
/// <param name="context">Контекст базы данных.</param>
public class BookingRepository(DatabaseContext context) : IBookingRepository
{
    /// <inheritdoc />
    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Bookings
            .AsNoTracking()
            .Include(b => b.Room)
            .Include(b => b.Charges)
            .Include(b => b.Slots)
            .SingleOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Booking?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Bookings
            .Include(b => b.Charges)
            .Include(b => b.Slots)
            .SingleOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> HasOverlappingSlotsAsync(
        Guid roomId,
        BookingPeriod period,
        CancellationToken cancellationToken = default)
    {
        var startsAt = period.StartsAtUtc;
        var endsAt = period.EndsAtUtc;

        return context.BookingSlots
            .AsNoTracking()
            .AnyAsync(
                s => s.RoomId == roomId && s.SlotStart >= startsAt && s.SlotStart < endsAt,
                cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        context.Bookings.Add(booking);

        return Task.CompletedTask;
    }

}
