using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Models;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Dal;
using RoomRental.Domain.Entities;
using RoomRental.Domain.ValueObjects;

namespace RoomRental.Infrastructure.Repositories;

/// <summary>
/// Хранилище залов.
/// </summary>
/// <param name="context">Контекст базы данных.</param>
public class RoomRepository(DatabaseContext context) : IRoomRepository
{
    private static readonly Expression<Func<Room, RoomDto>> ToDto = room => new RoomDto(
        room.Id,
        room.Name,
        room.Capacity,
        room.BaseHourPrice,
        room.Services
            .Where(s => s.DeletedAtUtc == null)
            .Select(s => new ServiceDto(s.Id, s.Name, s.Price))
            .ToList());

    /// <inheritdoc />
    public Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Rooms
            .AsNoTracking()
            .Include(r => r.Services)
            .SingleOrDefaultAsync(r => r.Id == id && r.DeletedAtUtc == null, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Room?> GetForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.Rooms
            .Include(r => r.Services)
            .SingleOrDefaultAsync(r => r.Id == id && r.DeletedAtUtc == null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RoomDto>> SearchAsync(
        SearchFilters filters,
        CancellationToken cancellationToken = default)
    {
        return await context.Rooms
            .AsNoTracking()
            .Where(r => r.DeletedAtUtc == null)
            .OrderBy(r => r.Name)
            .ThenBy(r => r.Id)
            .Skip(filters.Offset)
            .Take(filters.Limit)
            .Select(ToDto)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RoomDto>> SearchAvailableAsync(
        BookingPeriod period,
        int capacity,
        SearchFilters filters,
        CancellationToken cancellationToken = default)
    {
        var startsAt = period.StartsAtUtc;
        var endsAt = period.EndsAtUtc;

        return await context.Rooms
            .AsNoTracking()
            .Where(r => r.DeletedAtUtc == null && r.Capacity >= capacity)
            .Where(r => !context.BookingSlots
                .Any(s => s.RoomId == r.Id && s.SlotStart >= startsAt && s.SlotStart < endsAt))
            .OrderBy(r => r.BaseHourPrice)
            .ThenBy(r => r.Id)
            .Skip(filters.Offset)
            .Take(filters.Limit)
            .Select(ToDto)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task AddAsync(Room room, CancellationToken cancellationToken = default)
    {
        context.Rooms.Add(room);

        return Task.CompletedTask;
    }
}
