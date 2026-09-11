using Ardalis.GuardClauses;
using MediatR;
using RoomRental.Application.Abstraction.Dto;
using RoomRental.Application.Abstraction.Queries.Rooms;
using RoomRental.Application.Abstraction.Repositories;
using RoomRental.Domain.ValueObjects;

namespace RoomRental.Application.UseCases.Rooms;

/// <summary>
/// Возвращает залы, свободные на весь запрошенный период и подходящие по вместимости.
/// </summary>
public class SearchAvailableRoomsCase(
    IRoomRepository rooms,
    TimeProvider timeProvider)
    : IRequestHandler<SearchAvailableRoomsQuery, IReadOnlyList<RoomDto>>
{
    /// <inheritdoc />
    public Task<IReadOnlyList<RoomDto>> Handle(
        SearchAvailableRoomsQuery request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);
        Guard.Against.NegativeOrZero(request.Capacity);

        // Период проверяется теми же правилами, что и при бронировании:
        // иначе поиск покажет зал свободным на время, которое забронировать нельзя.
        var period = BookingPeriod.Create(request.Date, request.From, request.To);

        if (period.StartsAtUtc < timeProvider.GetUtcNow().UtcDateTime)
        {
            throw new ArgumentException("Нельзя искать залы на прошедшее время.");
        }

        return rooms.SearchAvailableAsync(period, request.Capacity, request.Filters, cancellationToken);
    }
}
